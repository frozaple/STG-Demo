local Enemy = require("actor/enemy")
local BulletEnum = require("actor/bulletenum")
local BulletShape = BulletEnum.Shape
local BulletColor = BulletEnum.Color

local Stage1_Enemy0 = Class(Enemy, function (self)
    Enemy.ctor(self, "Enemy/LittleFairyBlue")
    self:SetHp(300)

    self.shootTime1 = 0
    self.shootGap1 = 10
    self.shootParam1 = {BulletShape.Star, BulletColor.Blue, 2, 0.5}

    self.shootTime2 = 0
    self.shootGap2 = 36
    self.shootParam2 = {BulletShape.Kotama, BulletColor.Red, 8, 0.2}
end)

function Stage1_Enemy0:Update(timeScale)
    self:SetPosition(self.x, self.y - 2 * timeScale)
    self.shootTime1 = self.shootTime1 + timeScale
    if self.shootTime1 > self.shootGap1 then
        self:CircleShoot("linebullet", self.shootParam1, GStageMgr:GetRandom(0, 360), 6, 12)
        self.shootTime1 = self.shootTime1 - self.shootGap1
    end
    self.shootTime2 = self.shootTime2 + timeScale
    if self.shootTime2 > self.shootGap2 then
        self:FanShoot("linebullet", self.shootParam2, GStageMgr:GetPlayerAngle(self.x, self.y), 3, 40, 2)
        self.shootTime2 = self.shootTime2 - self.shootGap2
    end
end

return Stage1_Enemy0