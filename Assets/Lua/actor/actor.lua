local Actor = Class(function (self, name)
    self.obj = GStageMgr:SpawnObject(name)
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

    self.obj = nil
    self.transform = nil
end

return Actor