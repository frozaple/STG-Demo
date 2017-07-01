local Actor = require("actor/actor")

local Enemy = Class(Actor, function (self, name)
    Actor.ctor(self, name)

    if not self.ability then
        self.ability = self.obj:GetComponent(typeof(Enemy))
    end

    -----   ability value   -----
    self.hp = 0

    self.actionTime = 0
    self.shootGap = 3
    self.angle = 0
    self.angleSpeed = 0
end)

function Enemy:SetHp(hp)
    if self.hp ~= hp then
        self.hp = hp
        self.ability.hp = hp
    end
end

function Enemy:Update(timeScale)
    self.actionTime = self.actionTime + timeScale
    if self.actionTime > self.shootGap then
        self.angle = self.angle - self.angleSpeed
        if self.angle < 0 then self.angle = self.angle + 360 end

        self.angleSpeed = self.angleSpeed + 0.5
        if self.angleSpeed > 360 then self.angleSpeed = self.angleSpeed - 360 end

        self:CircleShoot("Enemy/Bullet/BulletKotama", self.angle, 8, 5)

        self.actionTime = self.actionTime - self.shootGap
    end
end

function Enemy:CircleShoot(bulletName, initAngle, way, speed)
    local angleDelta = 360 / way;
    for i = 1, way do
        local bullet = GGameMgr:SpawnBullet(bulletName)
        bullet:SetRotation(initAngle + i * angleDelta)
        bullet:SetPosition(self.x, self.y)
        bullet:SetSpeed(5)
    end
end

return Enemy