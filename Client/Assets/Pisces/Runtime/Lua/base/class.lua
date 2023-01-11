--[[/****************
 *@class name:		class
 *@description:		lua的class全局方法
 *@author:			selik0
 *@date:			2023-01-10 15:58:55
 *@version: 		V1.0.0
*************************************************************************/]] --
function clone(object, shallow)
    local lookup_table = {}
    local function _copy(object)
        if type(object) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end
        local new_table = {}
        lookup_table[object] = new_table
        for key, value in pairs(object) do
            new_table[_copy(key)] = _copy(value)
        end
        if shallow then
            return new_table
        else
            return setmetatable(new_table, getmetatable(object))
        end
    end

    return _copy(object)
end

local objectCount = 0
local function getObjectCount()
    objectCount = objectCount + 1
    return objectCount
end

function class(classname, super)
    local superType = type(super)
    local cls

    if superType ~= "function" and superType ~= "table" then
        superType = nil
        super = nil
    end

    if superType == "function" or (super and super.__ctype == 1) then
        -- inherited from native C# Object
        cls = {}
        local csobj

        if superType == "table" then
            -- copy fields from super
            for k, v in pairs(super) do cls[k] = v end
            cls.__create = super.__create
            cls.super    = super
        else
            cls.__create = super
            cls.ctor = function() end
        end

        cls.__cname = classname
        cls.__ctype = 1
        cls.__index = function(t, k)
            if csobj and csobj[k] then
                return csobj[k]
            end
            return rawget(cls, k)
        end
        cls.__newindex = function(t, k, v)
            if csobj and csobj[k] then
                csobj[k] = v
            end
            return rawset(cls, k, v)
        end

        function cls:new(...)
            csobj = cls.__create(...) --cs obj
            local instance = {}
            instance.id = getObjectCount()
            instance.class = cls
            setmetatable(instance, cls)
            instance:ctor(...)
            return instance
        end

    else
        -- inherited from Lua Object
        if super then
            cls = clone(super)
            cls.super = super
        else
            cls = { ctor = function() end }
        end

        cls.__cname = classname
        cls.__ctype = 2 -- lua
        cls.__index = cls

        function cls:new(...)
            local instance = {}
            instance.id = getObjectCount()
            instance.class = cls
            setmetatable(instance, cls)
            instance:ctor(...)
            return instance
        end
    end

    function cls:create(...)
        return cls:new(...)
    end

    function cls:isClass(clz)
        if clz and clz.__cname and clz.__cname == self.__cname then
            return true
        end
        return false
    end

    return cls
end
