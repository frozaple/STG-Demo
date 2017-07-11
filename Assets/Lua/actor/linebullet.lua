local Bullet = require("actor/bullet")

local LineBullet = Class(Bullet, function (self, shape, color, speed, delta)
    Bullet.ctor(self, shape, color)

    self.targetSpeed = speed
    self.speedDelta = delta
end)

function LineBullet:Update(timeScale)
    if self.speed > self.targetSpeed then
        local newSpeed = self.speed - self.speedDelta
        if newSpeed < self.targetSpeed then newSpeed = self.targetSpeed end
        self:SetSpeed(newSpeed)
    elseif self.speed < self.targetSpeed then
        local newSpeed = self.speed + self.speedDelta
        if newSpeed > self.targetSpeed then newSpeed = self.targetSpeed end
        self:SetSpeed(newSpeed)
    end
end

return LineBullet