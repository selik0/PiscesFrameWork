string = require("base.string")

local Number     = "Number"
local Bool       = "Bool"
local String     = "String"
local Vector2    = "Vector2"
local Vector3    = "Vector3"
local Vector4    = "Vector4"
local Color      = "Color"
local List       = "List"
local ObjectList = "ObjectList"
local ListList   = "ListList"
local Dict1      = "Dict1"
local Dict2      = "Dict2"
local TRUE       = "TRUE"

function tabletostring(tbl)
    if not tbl then
        return ""
    end
    local t = type(tbl)
    if t == "number" or t == "string" then
        return tbl
    elseif t == "boolean" then
        return tbl and "true" or "false"
    elseif t == "table" and tableIsNotNull(tbl) then
        local isNeedKey = false
        local len = #tbl
        local realyLen = 0
        for key, value in pairs(tbl) do
            realyLen = realyLen + 1
            if realyLen > len then
                isNeedKey = true
                break
            end
        end
        local str = "{ "
        for key, value in pairs(tbl) do
            if isNeedKey then
                local k = tabletostring(key)
                if type(k) == "number" then
                    k = "[" .. k .. "]"
                end
                str = str .. k .. " = " .. tabletostring(value) .. ", "
            else
                str = str .. tabletostring(value) .. ", "
            end
        end
        str = str .. " }"
        return str
    else
        return ""
    end
end

function excel2luaFunction(luaScriptName, dic)
    local memberMap = {
        { name = "id", typeName = "Number" }
    }
    for i = 2, dic[1].Count do
        local name = dic[2][i]
        local typeName = dic[3][i]
        if not string.IsNullOrEmpty(name) and not string.IsNullOrEmpty(typeName) then
            memberMap[i] = { name = name, typeName = typeName }
        end
        if string.find(typeName, "List") or string.find(typeName, "Dict") then
            local childMember = dic[4][i]
            local childTypeName = dic[5][i]
            if not string.IsNullOrEmpty(childMember) then
                memberMap[i].childMembers = string.split(childMember, "&")
            end
            if not string.IsNullOrEmpty(childTypeName) then
                memberMap[i].childTypeNames = string.split(childTypeName, "&")
            end
        end
    end
    local content = "local t = {\n"
    local config = { t = {} }
    for i = 8, dic.Count do
        local row = {}
        -- 第一列默认id
        local k, v = transitionCell(row, memberMap[1], dic[i][1],
            string.Format("{0}\t{1}\t{2}\t{3}", "%s", luaScriptName, i, 1))
        if row.id then
            content = content .. string.Format("[{0}] = {1} {2} = {3}, ", row.id, "{", k, v)
            -- 从第二列开始
            for j = 2, dic[i].Count do
                k, v = transitionCell(row, memberMap[j], dic[i][j],
                    string.Format("{0}\t{1}\t{2}\t{3}", "%s", luaScriptName, i, j))
                content = content .. string.Format("{0} = {1}, ", k, v)
            end
            content = content .. " }\n"
            config.t[row.id] = row
        else
            print("没找到id", luaScriptName, i)
        end
    end
    content = content .. "}\n return t"
    managerData(config, luaScriptName)
    return content
end

function managerData(config, luaScriptName)
    if not next(config.t) then
        print("表内没有数据", luaScriptName)
        return
    end
    local script = require(luaScriptName)
    if script.sort then
        table.sort(config.t, script.sort)
    end
    if script.check then
        script.check(config.t)
    end
    if not script.typeMap or not next(script.typeMap) then
        return
    end
    config.typeMap = {}
    for key, row in pairs(config.t) do
        local k = row[script.typeMap[1]]
        if config.typeMap[k] then
            table.insert(config.typeMap, key)
        else
            config.typeMap[k] = { key }
        end
    end
    if #script.typeMap >= 2 then
        local firstTypeList = config.typeMap

    end
end

function transitionCell(row, member, originValue, errorMsg)
    if not member then
        return
    end
    local value
    local name = member.name
    local typeName = member.typeName

    if typeName == Number or typeName == Bool or typeName == String or typeName == Vector2
        or typeName == Vector3 or typeName == Vector4 or typeName == Color then

        value = transitionBaseType(originValue, typeName, errorMsg)
    elseif typeName == List or typeName == Dict1 then
        local list = string2list(originValue)
        value = transitionList(list, typeName, member.childMembers, member.childTypeNames, errorMsg)
    elseif typeName == ListList or typeName == ObjectList or typeName == Dict2 then
        local list = string2list(originValue, true)
        value = transitionList(list, typeName, member.childMembers, member.childTypeNames, errorMsg)
    end
    row[name] = value
    return name, tabletostring(value)
end

function transitionBaseType(originValue, typeName, errorMsg)
    local value = originValue
    if string.IsNullOrEmpty(originValue) then
        return nil
    end
    if typeName == Number then
        value = tonumber(originValue)
    elseif typeName == String then
        value = string.Format('"{0}"', value)
    elseif typeName == Bool then
        value = originValue == "1"
    elseif typeName == Vector2 then
        local vec = string.split(originValue, ",")
        if next(vec) and #vec == 2 then
            value = string.Format("Vector2({0}, {1})", vec[1], vec[2])
        else
            error(string.format(errorMsg, "转换Vector2报错"))
        end
    elseif typeName == Vector3 then
        local vec = string.split(originValue, ",")
        if next(vec) and #vec == 3 then
            value = string.Format("Vector3({0}, {1}, {2})", vec[1], vec[2], vec[3])
        else
            error(string.format(errorMsg, "转换Vector3报错"))
        end
    elseif typeName == Vector4 then
        local vec = string.split(originValue, ",")
        if next(vec) and #vec == 4 then
            value = string.Format("Vector4({0}, {1}, {2}, {3})", vec[1], vec[2], vec[3], vec[4])
        else
            error(string.format(errorMsg, "转换Vector4报错"))
        end
    elseif typeName == Color then
        local vec = string.split(originValue, ",")
        if next(vec) and #vec == 4 then
            value = string.Format("Color({0}, {1}, {2}, {3})", vec[1], vec[2], vec[3], vec[4])
        else
            error(string.format(errorMsg, "转换Color报错"))
        end
    end
    return value
end

function string2list(originValue, isListList)
    local list
    if string.IsNullOrEmpty(originValue) then
        return list
    end
    list = string.split(originValue, "|")
    if isListList then
        for index, value in ipairs(list) do
            list[index] = string.split(value, "&")
        end
    end
    return list
end

function transitionList(list, typeName, childMembers, childTypeNames, errorMsg)
    if not tableIsNotNull(list) then
        return list
    end
    if typeName == List and tableIsNotNull(childTypeNames) and #childTypeNames == 1 then
        for index, v in ipairs(list) do
            list[index] = transitionBaseType(v, childTypeNames[index], errorMsg)
        end
    elseif typeName == ListList and tableIsNotNull(childTypeNames) and #childTypeNames == 1 then
        for i, mList in ipairs(list) do
            for j, v in ipairs(mList) do
                mList[j] = transitionBaseType(v, childTypeNames[1], errorMsg)
            end
        end
    elseif typeName == ObjectList and tableIsNotNull(childMembers) and tableIsNotNull(childTypeNames) and
        #childTypeNames == #childMembers then
        for i, mList in ipairs(list) do
            if #childMembers ~= #mList then
                error(string.format(errorMsg, "转换ObjectList报错"))
                return nil
            end
            local map = {}
            for j, v in ipairs(mList) do
                print("ObjectList", childMembers[j], childTypeNames[j], v)
                map[childMembers[j]] = transitionBaseType(v, childTypeNames[j], errorMsg)
            end
            list[i] = map
        end
    elseif typeName == Dict1 and tableIsNotNull(childTypeNames) and #childTypeNames == 1 then
        local map = {}
        for index, v in ipairs(list) do
            map[transitionBaseType(v, childTypeNames[1], errorMsg)] = index
        end
        table.sort(map, function(a, b) return a < b end)
        list = map
    elseif typeName == Dict2 and tableIsNotNull(childTypeNames) and #childTypeNames == 2 then
        for i, mList in ipairs(list) do
            local map = {}
            map[transitionBaseType(mList[1], childTypeNames[2], errorMsg)] = transitionBaseType(mList[2],
                childTypeNames[2], errorMsg)
            list[i] = map
        end
    end
    return list
end

function tableIsNotNull(tbl)
    return tbl and type(tbl) == "table" and next(tbl)
end
