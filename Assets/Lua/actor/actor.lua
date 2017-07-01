local Actor = Class(function (self, name)
    self.name = name
    self.obj = GObjectMgr:SpawnObject(name)
    self.instanceID = self.obj:GetInstanceID()
    self.ability = GObjectMgr:GetCompCache(self.instanceID)
    self.transform = self.obj.transform

    -----   transform value   -----
    self.x = 0
    self.y = 0
    self.rot = 0

    self.valid = true
end)

function Actor:SetPosition(x, y)
    if self.x ~= x or self.y ~= y then
        self.x = x
        self.y = y
        self.transform.localPosition = Vector3(x, y, 0)
    end
end

function Actor:SetRotation(rot)
    if self.rot ~= rot then
        self.rot = rot
        self.transform.eulerAngles = Vector3(0, 0, rot)
    end
end

function Actor:Destroy()
    self.valid = false
    GObjectMgr:DespawnObject(self.name, self.obj)
    GObjectMgr:AddCompCache(self.instanceID, self.ability)

    self.obj = nil
    self.transform = nil
    self.ability = nil
end

return Actor