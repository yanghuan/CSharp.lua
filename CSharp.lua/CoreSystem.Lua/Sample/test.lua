--require("socket")
--local conf = { time = socket.gettime }
require("All")(nil, conf)
collectgarbage("collect")
print(collectgarbage("count"))

local function test(f, name) 
    print("-----------------------------", name)
    f()
    print("\n")
end

local function printList(list)
    assert(list)
    local t = {}
    for _, i in System.each(list) do
        table.insert(t, i:ToString())
    end
    print(table.concat(t, " "))
end

local function testDateTimeAndTimeSpan() 
    local date = System.DateTime.getNow()
    print(date:getTicks())
    print(date:ToString(), date:getYear(), date:getMonth(), date:getDay(), date:getHour(), date:getMinute(), date:getSecond())
    
    local ts = System.TimeSpan.FromSeconds(20)
    print(ts:ToString())
    
    date = date + System.TimeSpan.FromDays(2)
    print(date:ToString())
    
    local baseTime = System.DateTime(1970, 1, 1) 
    print(baseTime:ToString())
    print(baseTime:AddMilliseconds(1458032204643):ToString())
end

local function testArray() 
    local arr = System.Array(System.Int):new(10)
    print(arr:ToString(), #arr)
    printList(arr)
    arr:set(0, 2)
    arr:set(6, 4)
    printList(arr)
    print(arr:get(0), arr:get(6), arr:get(9))
end

local function testList()
    local list = System.List(System.Int)()
    list:Add(20)
    list:Add(15)
    list:Add(6)
    print(list:ToString(), #list)
    printList(list)
    local subList = list:GetRange(1, 2)
    printList(subList)
    list:set(1, 8)
    list:Sort()
    printList(list)
    print(list:Contains(10), list:Contains(15), list:IndexOf(20))
    list:RemoveAll(function(i) return i >= 10 end)
    print(#list, list:get(1))
    printList(list)
end

local function testDictionary()
    local dict = System.Dictionary(System.String, System.Int)()
    dict:Add("a", 1)
    dict:Add("b", 12)
    dict:Add("c", 25)
    dict:Add("d", 30)
    for _,  pair in System.each(dict) do
        print(pair.key, pair.value)
    end
end

local function testYeild()
    local enumerable = function (begin, _end) 
        return System.yieldIEnumerable(function (begin, _end)
            while begin < _end do
                System.yieldReturn(begin)
                begin = begin + 1
            end
        end, System.Int, begin, _end)
    end
    local e = enumerable(1, 10)
    printList(e)
    printList(e)
end

local function testDelegate()
    local prints = ""
    local function printExt(s)
        prints = prints .. s
        print(s)
    end
    local function assertExt(s)
        assert(prints == s, s)
        prints = ""
    end
    local d1 = function() printExt("d1") end
    local d2 = function() printExt("d2") end
    local d3 = function() printExt("d3") end
    System.combine(nil, d1)()
    assertExt("d1")
    print("--")
    
    System.combine(d1, nil)()
    assertExt("d1")
    print("--")
    
    System.combine(d1, d2)()
    assertExt("d1d2")
    print("--")
    
    System.combine(d1, System.combine(d2, d3))()
    assertExt("d1d2d3")
    print("--")
    
    System.combine(System.combine(d1, d2), System.combine(d2, d3))()
    assertExt("d1d2d2d3")
    print("--")
    
    System.remove(System.combine(d1, d2), d1)()
    assertExt("d2")
    print("--")
    
    System.remove(System.combine(d1, d2), d2)()
    assertExt("d1")
    print("--")
    
    System.remove(System.combine(System.combine(d1, d2), d1), d1)()
    assertExt("d1d2")
    print("--")
    
    System.remove(System.combine(System.combine(d1, d2), d3), System.combine(d1, d2))()
    assertExt("d3")
    print("--")
    
    System.remove(System.combine(System.combine(d1, d2), d3), System.combine(d2, d1))()
    assertExt("d1d2d3")
    print("--")
    
    fn0 = System.combine(System.combine(d1, d2), System.combine(System.combine(d3, d1), d2))
    fn1 = System.combine(d1, d2)
    System.remove(fn0, fn1)()
    assertExt("d1d2d3")
    
    print("--")
    local i = System.remove(System.combine(d1, d2), System.combine(d1, d2))
    print(i == nil)
end

local function testLinq()
    local Linq = System.Linq.Enumerable
    local list = System.List(System.Int)()
    list:Add(1) list:Add(2) list:Add(3) list:Add(4) list:Add(5) list:Add(6) list:Add(7) list:Add(8)
    printList(Linq.Where(list, function(i) return i >= 4 end))
    printList(Linq.Take(list, 4))
    printList(Linq.Select(list, function(i) return i + 2 end, System.Int))
    print(Linq.Min(list), Linq.Max(list))
    print(Linq.ElementAtOrDefault(Linq.Where(list, function(i) return i <= 4 end), 5))
    local ll = Linq.Where(list, function(i) return i <= 4 end)
    print(Linq.Count(ll))
    Linq.Any(ll)
    print(Linq.Count(ll))
    
end

local function testType()
    local ins = 2
    print(System.is(ins, System.Double))
    local t = ins:GetType()
    print(t:getName())
    print(System.is("ddd", System.String))
    print(System.as("ddd", System.String))
    print(System.cast(System.String, "ddd"))
end

local function testConsole()
    print("enter your name")
    local name = System.Console.ReadLine()
    print("enter your age")
    local age = System.Console.ReadLine()
    System.Console.WriteLine("name {0}, age {1}", name, age)
end

local function testIO()
    local path = "iotest.txt"
    local s = "hero, brige.lua\nIO"
    System.File.WriteAllText(path, s)
    local text = System.File.ReadAllText(path)
    assert(text == s)
    os.remove(path)
end

test(testDateTimeAndTimeSpan, "DateTime & TimeSpan")
test(testArray, "Array")
test(testList, "List")
test(testDictionary, "Dictionary")
test(testYeild, "Yeild")
test(testDelegate, "Delegate")
test(testLinq, "Linq")
test(testType, "Type")
--test(testConsole, "Console")
test(testIO, "IO")




