--[[
groupMap表示根据字段生产分组列表, 第一个字段表示一级分类， 第二个字段表示二级分类,以此类推
生成的列表示例：
groupMap = {
    [1] = {
        {11} = {11000, 11001},
        {12} = {21000, 21001},
    }
    [2] = {
        [21] = {12000, 12001},
        [22] = {22000, 22001},
    }
}
sort表示排序方法
check表示检查数据的方式，防止策划配置错误

local t = {
    groupMap = { "type1", "type2" },
    sort = function(a, b)
        return a.id < b.id
    end,
    -- tbl表示当前表的所有数据
    check = function(tbl)

    end
}

return t
]]
local t = {
    groupMap = { "testNumber1", "testNumber2", "testNumber3" },
    -- sort = function(a, b)
    --     return a.id < b.id
    -- end,
    -- check = function(tbl)
    --     -- todo 检查数据
    -- end
}

return t
