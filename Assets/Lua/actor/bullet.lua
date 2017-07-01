local Actor = require("actor/actor")

local Bullet = Class(Actor, function (self, name)
    Actor.ctor(self, name)

    if not self.ability then
        self.ability = self.obj:GetComponent(typeof(EnemyBullet))
    end

    -----   ability value   -----
    self.speed = 0
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