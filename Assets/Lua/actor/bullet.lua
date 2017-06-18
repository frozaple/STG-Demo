local Actor = require("actor/actor")

local Bullet = Class(Actor, function (self, name)
    Actor.ctor(self, name)

    self.ability = self.obj:GetComponent(typeof(EnemyBullet))

    -----   ability value   -----
    self.speed = 0

    self.customUpdate = false
end)

function Bullet:SetSpeed(speed)
    if self.speed ~= speed then
        self.speed = speed
        self.ability.speed = speed
    end
end

function Bullet:Update(timeScale)
end

return Bullet