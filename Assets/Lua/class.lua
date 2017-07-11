function Class(base, ctor)
    if type(base) == "function" then
        ctor = base
        base = nil
    end

    local class = {ctor = ctor, base = base}
    class.__index = class

    local meta = {__index = base}
    meta.__call = function (_, ...)
        local instance = {}
        setmetatable(instance, class)
        -- should call base class's constructor by itself
        class.ctor(instance, ...)
        return instance
    end

    setmetatable(class, meta)
    return class
end