local Enemy = require("actor/enemy")
local Bullet = require("actor/bullet")

local GameManager = Class(function (self)
    self.script = nil

    self.enemy = {}
    self.bullet = {}

    self.ID2Enemy = {}
    self.ID2Bullet = {}
end)

function GameManager:SpawnEnemy(enemyName)
    local enemyActor = Enemy(enemyName)
    table.insert(self.enemy, enemyActor)
    self.ID2Enemy[enemyActor.instanceID] = enemyActor
    return enemyActor
end

function GameManager:SpawnBullet(bulletName)
    local bulletActor = Bullet(bulletName)
    table.insert(self.bullet, bulletActor)
    self.ID2Bullet[bulletActor.instanceID] = bulletActor
    return bulletActor
end

local function ExecUpdate(list, timeScale)
    for i = #list, 1, -1 do
        local actor = list[i]
        if actor.valid then
            actor:Update(timeScale)
        else
            table.remove(list, i)
        end
    end
end

function GameManager:Update(timeScale)
    ExecUpdate(self.enemy, timeScale)
    ExecUpdate(self.bullet, timeScale)
end

local function ExecDespawn(ID2Actor, instanceID)
    if ID2Actor[instanceID] then
        local actor = ID2Actor[instanceID]
        actor:Destroy()
        ID2Actor[instanceID] = nil
    end
end

function GameManager:DespawnActor(instanceID)
    ExecDespawn(self.ID2Enemy, instanceID)
    ExecDespawn(self.ID2Bullet, instanceID)
end

return GameManager
