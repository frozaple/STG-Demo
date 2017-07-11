local Bullet = require("actor/bullet")
local LevelScript = require("level/levelscript")

local GameManager = Class(function (self)
    self.script = nil
    self.enemyClass = {}
    self.bulletClass = {}

    self.enemy = {}
    self.bullet = {}

    self.ID2Enemy = {}
    self.ID2Bullet = {}
end)

function GameManager:LoadScript()
    self.script = LevelScript()
    self.script:Initialize()
end

local function ExecSpawn(classList, actorList, ID2Actor, actorName, ...)
    local actorClass = classList[actorName]
    if not actorClass then
        actorClass = require("actor/" .. actorName)
        classList[actorName] = actorClass
    end
    local actor = actorClass(...)
    table.insert(actorList, actor)
    ID2Actor[actor.instanceID] = actor
    return actor
end

function GameManager:SpawnEnemy(enemyName, ...)
    return ExecSpawn(self.enemyClass, self.enemy, self.ID2Enemy, enemyName, ...)
end

function GameManager:SpawnBullet(bulletName, ...)
    return ExecSpawn(self.bulletClass, self.bullet, self.ID2Bullet, bulletName, ...)
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
    self.script:Update(timeScale)
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
