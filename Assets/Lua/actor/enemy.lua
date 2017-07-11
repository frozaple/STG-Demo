local Actor = require("actor/actor")

local Enemy = Class(Actor, function (self, name)
    Actor.ctor(self, name)

    if not self.ability then
        self.ability = self.obj:GetComponent(typeof(Enemy))
    end

    -----   ability value   -----
    self.hp = 0
end)

function Enemy:SetHp(hp)
    if self.hp ~= hp then
        self.hp = hp
        self.ability.hp = hp
    end
end

local function SpawnBullet(bulletName, params)
    if params then
        return GGameMgr:SpawnBullet(bulletName, unpack(params))
    else
        return GGameMgr:SpawnBullet(bulletName)
    end
end

function Enemy:CircleShoot(bulletName, params, initAngle, way, speed)
    local angleDelta = 360 / way;
    for i = 1, way do
        local bullet = SpawnBullet(bulletName, params)
        bullet:SetRotation(initAngle + i * angleDelta)
        bullet:SetPosition(self.x, self.y)
        bullet:SetSpeed(speed)
    end
end

function Enemy:FanShoot(bulletName, params, centerAngle, way, angleDelta, speed)
    local initAngle = centerAngle - (way + 1) * angleDelta / 2
    for i = 1, way do
        local bullet = SpawnBullet(bulletName, params)
        bullet:SetRotation(initAngle + i * angleDelta)
        bullet:SetPosition(self.x, self.y)
        bullet:SetSpeed(speed)
    end
end

return Enemy