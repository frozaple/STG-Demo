local LevelEvent = Class(function (self, time, func, loop, gap)
    self.valid = true
    self.execTime = time
    self.execFunc = func
    self.execCount = 0
    self.loopCount = loop
    self.loopGap = gap
end)

function LevelEvent:Run(time)
    while time >= self.execTime do
        self.execFunc(self.execCount)
        self.execCount = self.execCount + 1
        self.loopCount = self.loopCount - 1
        if self.loopCount > 0 then
            self.execTime = self.execTime + self.loopGap
        else
            self.valid = false
            break
        end
    end
end

local LevelScript = Class(function (self)
    self.time = 0
    self.eventList = {}
end)

function LevelScript:Update(timeScale)
    self.time = self.time + timeScale

    for i = #self.eventList, 1, -1 do
        local event = self.eventList[i]
        event:Run(self.time)
        if not event.valid then
            table.remove(self.eventList, i)
        end
    end
end

function LevelScript:AddEvent(time, func, loop, gap)
    local event = LevelEvent(time, func, loop or 1, gap or 0)
    table.insert(self.eventList, event)
end

function LevelScript:Initialize()
    self:AddEvent(0, function (i)
        local enemy = GGameMgr:SpawnEnemy("stage1_enemy0")
        enemy:SetPosition(i * 32 + 32, 480)
    end, 20, 30)
    self:AddEvent(600, function (i)
        local enemy = GGameMgr:SpawnEnemy("stage1_enemy0")
        enemy:SetPosition(608 - i * 32, 480)
    end, 20, 30)
end

return LevelScript