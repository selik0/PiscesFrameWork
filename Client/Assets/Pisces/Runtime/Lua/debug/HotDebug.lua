require("excel2lua")
require("base.serialize")
-- local outMap = {}
-- local dimensionArr = {
--     { [1] = 1, [2] = 1, [3] = 1, [4] = 1 },
--     { [5] = 1, [6] = 1, [7] = 1, [8] = 1 },
--     { [9] = 1, [10] = 1, [11] = 1, [12] = 1 },
-- }

-- local groupArr = { "test1", "test2", "test3" }

-- local tbl = createMultipleDimensionsMap(outMap, outMap, dimensionArr, 0)
-- print(tableToString(tbl))

local t = {
    {
        [8] = { [12] = {}, [9] = {}, [10] = {}, [11] = {}, },
        [5] = { [12] = {}, [9] = {}, [10] = {}, [11] = {}, },
        [6] = { [12] = {}, [9] = {}, [10] = {}, [11] = {}, },
        [7] = { [12] = {}, [9] = {}, [10] = {}, [11] = {}, },
    },
    {
        [8] = { [12] = {}, [9] = {}, [10] = {}, [11] = {}, },
        [5] = { [12] = {}, [9] = {}, [10] = {}, [11] = {}, },
        [6] = { [12] = {}, [9] = {}, [10] = {}, [11] = {}, },
        [7] = { [12] = {}, [9] = {}, [10] = {}, [11] = {}, },
    },
    {
        [8] = { [12] = {}, [9] = {}, [10] = {}, [11] = {}, },
        [5] = { [12] = {}, [9] = {}, [10] = {}, [11] = {}, },
        [6] = { [12] = {}, [9] = {}, [10] = { 5, 6 }, [11] = {}, },
        [7] = { [12] = {}, [9] = {}, [10] = {}, [11] = {}, },
    },
    {
        [8] = { [12] = {}, [9] = {}, [10] = {}, [11] = {}, },
        [5] = { [12] = {}, [9] = {}, [10] = {}, [11] = {}, },
        [6] = { [12] = {}, [9] = {}, [10] = {}, [11] = {}, },
        [7] = { [12] = {}, [9] = {}, [10] = {}, [11] = {}, },
    },
}

-- print(tableToString(getMultipleDimensionsMap(t, { 3, 6, 10 })))

print(serialize(t))
