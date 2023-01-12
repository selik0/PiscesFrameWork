local insert = table.insert
local concat = table.concat
local format = string.format
local tostring = tostring
local type = type
local foreach = function(t, func)
    for k, v in pairs(t) do
        func(k, v)
    end
end
-- 函数调用的层级
DEBUG_LEVEL = 3
-- 序列化Table
SERIALIZE_TABLE = true
function serialize(t)
    local mark = {}
    local assign = {}

    local tb_cache = {}
    local function tb(len)
        if tb_cache[len] then
            return tb_cache[len]
        end
        local ret = ''
        while len > 1 do
            ret = ret .. '\t'
            len = len - 1
        end
        tb_cache[len] = ret
        return ret
    end

    local function table2str(t, parent, deep)
        if type(t) == "table" and t.__tostring then
            return tostring(t)
        end

        deep = deep or 0
        mark[t] = parent
        local ret = {}
        foreach(t, function(f, v)
            local k = type(f) == "number" and "[" .. f .. "]" or tostring(f)
            local t = type(v)
            if t == "userdata" or t == "function" or t == "thread" or t == "proto" or t == "upval" then
                insert(ret, format("%s = %q", k, tostring(v)))
            elseif t == "table" then
                local dotkey = parent .. (type(f) == "number" and k or "." .. k)
                if mark[v] then
                    insert(assign, dotkey .. " = " .. mark[v])
                else
                    insert(ret, format("%s = %s", k, table2str(v, dotkey, deep + 1)))
                end
            elseif t == "string" then
                insert(ret, format("%s = %q", k, v))
            elseif t == "number" then
                if v == math.huge then
                    insert(ret, format("%s = %s", k, "math.huge"))
                elseif v == -math.huge then
                    insert(ret, format("%s = %s", k, "-math.huge"))
                else
                    insert(ret, format("%s = %s", k, tostring(v)))
                end
            else
                insert(ret, format("%s = %s", k, tostring(v)))
            end
        end)
        return "{\n" .. tb(deep + 2) .. concat(ret, ",\n" .. tb(deep + 1)) .. '\n' .. tb(deep) .. "}"
    end

    if type(t) == "table" then
        if t.__tostring then
            return tostring(t)
        end
        if SERIALIZE_TABLE then
            local str = format("\n%s%s", table2str(t, "_"), concat(assign, " "))
            return str
        else
            -- 若是class
            if t.__cname then
                return tostring(t.__cname)
            end

            return format("\n%s[%d]", tostring(t), #t)
        end
    else
        return tostring(t)
    end
end
