local table = table
local _insert = table.insert
local _concat = table.concat

local unity_string = CS.System.String
local lua_string = string
local _substr = lua_string.sub
local _find = lua_string.find

local StringUtility = {}

StringUtility.__index = function(t, k)
    local var = rawget(lua_string, k)
    if var ~= nil then
        return var
    end
    return rawget(unity_string, k)
end

function StringUtility.split(str, delimiter, plain)
    if not str then return {} end

    if (delimiter == '') then return false end
    if plain == nil then plain = true end
    local pos, arr = 0, {}
    -- for each divider found
    for st, sp in function() return _find(str, delimiter, pos, plain) end do
        _insert(arr, _substr(str, pos, st - 1))
        pos = sp + 1
    end
    _insert(arr, _substr(str, pos))
    return arr
end

setmetatable(StringUtility, StringUtility)

return StringUtility
