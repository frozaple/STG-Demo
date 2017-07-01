local ObjectManager = Class(function (self)
    self.objectPools = {}
    self.compCache = {}
end)

function ObjectManager:SpawnObject(objName)
    if self.objectPools[objName] and #self.objectPools[objName] > 0 then
        local obj = self.objectPools[objName][1]
        table.remove(self.objectPools[objName], 1)
        obj:SetActive(true)
        return obj
    end

    return GStageMgr:SpawnObject(objName)
end

function ObjectManager:DespawnObject(objName, obj)
    if not self.objectPools[objName] then
        self.objectPools[objName] = {}
    end
    obj:SetActive(false)
    table.insert(self.objectPools[objName], obj)
end

function ObjectManager:GetCompCache(instanceID)
    local comp = self.compCache[instanceID]
    self.compCache[instanceID] = nil
    return comp
end

function ObjectManager:AddCompCache(instanceID, comp)
    self.compCache[instanceID] = comp
end

return ObjectManager
