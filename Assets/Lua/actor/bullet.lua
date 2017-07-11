local Actor = require("actor/actor")
local BulletEnum = require("actor/bulletenum")
local BulletShape = BulletEnum.Shape
local BulletColor = BulletEnum.Color

local Bullet = Class(Actor, function (self, shape, color)
    Actor.ctor(self, "Enemy/Bullet/EnemyBullet")

    if not self.ability then
        self.ability = self.obj:GetComponent(typeof(EnemyBullet))
    end
    self:SetAppearance(shape, color)

    -----   ability value   -----
    self.speed = 0
end)

function Bullet:SetAppearance(shape, color)
    if self.shape ~= shape or self.color ~= color then
        self.shape = shape
        self.color = color
        self.ability:SetAppearance(shape, color)
        self.ability:SetSelfRotate(self.shape == BulletShape.Star)
    end
end

function Bullet:SetRotation(rot)
    if self.rot ~= rot then
        self.rot = rot
        self.ability:SetRotation(rot)
    end
end

function Bullet:SetSpeed(speed)
    if self.speed ~= speed then
        self.speed = speed
        self.ability.speed = speed
    end
end


return Bullet