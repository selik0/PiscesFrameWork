string = require("base.string")
require("base.serialize")
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
local TRUE       = "1"

function excel2luaFunction(luaScriptName, dic)
    -- 第一步获取所有的字段
    local isGetAllMember, memberMap = firstStep_GetAllMember(dic)
    if not isGetAllMember then
        print("List,Dict的子对象的类型对不上转表失败", luaScriptName)
        return
    end

    -- 第二步获取excel表的数据
    local config = {}
    config.t = secondStep_GetBaseData(dic, memberMap, luaScriptName)

    -- 第三步处理excel配置表对应的lua配置方法
    thirdStep_DoLuaConfig(config, luaScriptName)

    return fourStep_ToString(config, memberMap, dimensionArr)
end

-- 第一步获取所有的字段
function firstStep_GetAllMember(dic)
    local memberMap = {
        { idx = 1, name = "id", typeName = "Number" }
    }
    -- 第一列默认为id, 获取字段
    for i = 2, dic[1].Count do
        table.insert(memberMap, getMember(i, dic[2][i], dic[3][i], dic[4][i], dic[5][i]))
    end
    local isSuc = true
    -- 字段类型不正确直接返回
    for key, value in pairs(memberMap) do
        if (string.find(value.typeName, "List") or string.find(value.typeName, "Dict")) and not value.childMembers then
            isSuc = false
        end
    end
    return isSuc, memberMap
end

-- 第二步获取excel表的数据
function secondStep_GetBaseData(dic, memberMap, luaScriptName)
    -- 先获取原始的文本数据
    local t = {}
    for i = 8, dic.Count do
        local row = {}
        -- 第一列id不为空才转换数据
        if not string.IsNullOrEmpty(dic[i][1]) then
            for _, member in ipairs(memberMap) do
                local errorMsg = '%s' .. string.format(" 文件名%s,  行%s,  列%s", luaScriptName, i, member.idx)
                row[member.name] = transitionBaseType(dic[i][member.idx], member, errorMsg)
            end
            t[row.id] = row
        else
            print("id为空", luaScriptName, i)
        end
    end
    return t
end

-- 第三步处理excel配置表对应的lua配置方法
function thirdStep_DoLuaConfig(config, luaScriptName)
    if not next(config.t) then
        print("表内没有数据", luaScriptName)
        return
    end
    local script = require(luaScriptName)
    if script.sort then
        table.sort(config.t, script.sort)
    end
    if not tableIsNotNull(script.groupMap) then
        return
    end

    local groupMap = {}
    local temp = {}
    for k, name in ipairs(script.groupMap) do
        temp[k] = {}
    end
    for k, v in pairs(config.t) do
        for k, name in ipairs(script.groupMap) do
            temp[k][v[name]] = 1
        end
    end

    groupMap = createMultipleDimensionsMap(groupMap, groupMap, temp, 0)

    for k, v in pairs(config.t) do
        local keyList = {}
        for index, name in ipairs(script.groupMap) do
            keyList[index] = v[name]
        end
        local tempOutMap = getMultipleDimensionsMap(groupMap, keyList)
        table.insert(tempOutMap, k)
    end
    config.groupMap = groupMap
end

-- 第四步 数据转成string准备写入文件
function fourStep_ToString(config, memberMap)
    local content = ""
    if tableIsNotNull(config.t) then
        for key, row in pairs(config.t) do
            local rowContent = ""
            for index, member in ipairs(memberMap) do
                rowContent = rowContent .. string.format("%s = %s, ", member.name, tableToString(row[member.name]))
            end
            content = content .. string.format("\t[%s] = { %s },\n", key, rowContent)
        end
        content = string.format("local t = {\n%s}\n", content)
    end

    if tableIsNotNull(config.groupMap) then
        content = content .. "local groupMap = " .. serialize(config.groupMap)
    end

    return content
end

-- 检查数据入口
function checkLuaData()
    
end

function getMember(i, memberName, memberTypeName, childMemberNameStr, childMemberTypeStr)
    if string.IsNullOrEmpty(memberName) or string.IsNullOrEmpty(memberTypeName) then
        return
    end
    local member = { idx = i, name = memberName, typeName = memberTypeName }
    -- 子类型没数据直接返回
    if string.IsNullOrEmpty(childMemberTypeStr) then
        return member
    end
    if memberTypeName == List or memberTypeName == ListList or memberTypeName == Dict1 then
        member.childMembers = { { typeName = childMemberTypeStr } }
    elseif memberTypeName == ObjectList then
        local list1 = string.split(childMemberNameStr, "&")
        local list2 = string.split(childMemberTypeStr, "&")
        if #list1 == #list2 and #list1 > 0 then
            member.childMembers = {}
            for index, str in ipairs(list1) do
                member.childMembers[index] = { name = str, typeName = list2[index] }
            end
        end
    elseif memberTypeName == Dict2 then
        local list1 = string.split(childMemberTypeStr, "&")
        if #list1 == 2 then
            member.childMembers = { { typeName = list1[1] }, { typeName = list1[2] } }
        end
    end
    return member
end

function transitionBaseType(originValue, member, errorMsg)
    local value = originValue
    if string.IsNullOrEmpty(originValue) then
        return nil
    end
    local typeName = member.typeName
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
            print(string.format(errorMsg, "转换Vector2报错"))
        end
    elseif typeName == Vector3 then
        local vec = string.split(originValue, ",")
        if next(vec) and #vec == 3 then
            value = string.Format("Vector3({0}, {1}, {2})", vec[1], vec[2], vec[3])
        else
            print(string.format(errorMsg, "转换Vector3报错"))
        end
    elseif typeName == Vector4 then
        local vec = string.split(originValue, ",")
        if next(vec) and #vec == 4 then
            value = string.Format("Vector4({0}, {1}, {2}, {3})", vec[1], vec[2], vec[3], vec[4])
        else
            print(string.format(errorMsg, "转换Vector4报错"))
        end
    elseif typeName == Color then
        local vec = string.split(originValue, ",")
        if next(vec) and #vec == 4 then
            value = string.Format("Color({0}, {1}, {2}, {3})", vec[1], vec[2], vec[3], vec[4])
        else
            print(string.format(errorMsg, "转换Color报错"))
        end
    elseif typeName == List then
        local list = string.split(originValue, "|")
        for index, str in ipairs(list) do
            list[index] = transitionBaseType(str, member.childMembers[1], errorMsg)
        end
        value = list
    elseif typeName == ListList then
        local list = string.split(originValue, "|")
        for index, str in ipairs(list) do
            list[index] = string.split(str, "&")
        end
        for _, mList in ipairs(list) do
            for index, str in ipairs(mList) do
                mList[index] = transitionBaseType(str, member.childMembers[1], errorMsg)
            end
        end
        value = list
    elseif typeName == ObjectList then
        local list = string.split(originValue, "|")
        for index, str in ipairs(list) do
            list[index] = string.split(str, "&")
        end
        for idx, mList in ipairs(list) do
            local tempList = {}
            for index, str in ipairs(mList) do
                tempList[member.childMembers[index].name] = transitionBaseType(str, member.childMembers[index], errorMsg)
            end
            list[idx] = tempList
        end
        value = list
    elseif typeName == Dict1 then
        local list = string.split(originValue, "|")
        value = {}
        for index, str in ipairs(list) do
            value[transitionBaseType(str, member.childMembers[1], errorMsg)] = index
        end
    elseif typeName == Dict2 then
        local list = string.split(originValue, "|")
        for index, str in ipairs(list) do
            list[index] = string.split(str, "&")
        end
        value = {}
        for index, mList in ipairs(list) do
            if #mList == 2 then
                value[transitionBaseType(mList[1], member.childMembers[1], errorMsg)] = transitionBaseType(mList[2],
                    member.childMembers[2], errorMsg)
            else
                print(string.format(errorMsg, "转换Dict报错"))
            end
        end
    else
        print(string.format(errorMsg, string.format("不支持格式%s %s", member.name, typeName)))
    end
    return value
end

function createMultipleDimensionsMap(reallyOutMap, outMap, dimensionArr, num)
    if num == #dimensionArr then
        return outMap
    end
    num = num + 1
    local temp = dimensionArr[num]
    for k, _ in pairs(temp) do
        local tempOutMap = {}
        outMap[k] = tempOutMap
        createMultipleDimensionsMap(reallyOutMap, tempOutMap, dimensionArr, num)
    end
    return reallyOutMap
end

function getMultipleDimensionsMap(dimensionMap, keyArr)
    local outMap = dimensionMap
    for index, value in ipairs(keyArr) do
        outMap = outMap[value]
    end
    return outMap
end

function tableToString(tbl)
    if not tbl then
        return ""
    end
    local t = type(tbl)
    if t == "number" or t == "string" then
        return tbl
    elseif t == "boolean" then
        return tostring(tbl)
    elseif t == "table" then
        if not tableIsNotNull(tbl) then
            return "{}"
        end
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
                local k = tableToString(key)
                if type(k) == "number" then
                    k = "[" .. k .. "]"
                end
                str = str .. k .. " = " .. tableToString(value) .. ", "
            else
                str = str .. tableToString(value) .. ", "
            end
        end
        str = str .. "}"
        return str
    else
        return ""
    end
end

function formatTable(tbl, nowTier, allTier)
    if type(tbl) ~= "table" then
        return tableToString(tbl)
    end

    local content = ""
    for key, row in pairs(tbl) do
        local rowContent = ""
        for index, member in ipairs(memberMap) do
            rowContent = rowContent .. string.format("%s = %s, ", member.name, tableToString(row[member.name]))
        end
        content = content .. string.format("\t[%s] = { %s },\n", key, rowContent)
    end
    content = string.format("local t = {\n%s}\n", content)
    for key, value in pairs(tbl) do
            
    end
end

function tableIsNotNull(tbl)
    return tbl and type(tbl) == "table" and next(tbl)
end
