local Enemy = require("actor/enemy")
local Bullet = require("actor/bullet")

local GameManager = Class(function (self)
    self.script = nil
    self.enemy = {}
    self.bullet = {}
end)

function GameManager:SpawnEnemy(enemyName)
    local enemyActor = Enemy(enemyName)
    table.insert(self.enemy, enemyActor)
    return enemyActor
end

function GameManager:SpawnBullet(bulletName)
    local bulletActor = Bullet(bulletName)
    if bulletActor.customUpdate then
        table.insert(self.bullet, bulletActor)
    end
    return bulletActor
end

function GameManager:Update(timeScale)
    for _, enemy in ipairs(self.enemy) do
        enemy:Update(timeScale)
    end
    for _, bullet in ipairs(self.bullet) do
        bullet:Update(timeScale)
    end
end

return GameManager