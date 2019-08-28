#r "NPSBDummyLib.dll"
#r "nuget: System.Threading.Channels, 4.5.0"
#r "nuget: System.Threading.Tasks.Extensions, 4.5.3"
#r "nuget: MessagePack, 1.7.3"
#r "nuget: NLog, 4.6.7"

using NPSBDummyLib;

var testUniqueIndex = DateTime.Now.Ticks;
var config = new TestConfig
{
	ActionIntervalTime = 100,
	ActionRepeatCount = 3,
	DummyIntervalTime = 0,
	LimitActionTime = 90000,
	RoomNumber = 1,
	ChatMessage = "ScriptTest",
};

DummyManager.SetDummyInfo = new DummyInfo
{
	RmoteIP = "127.0.0.1",
	RemotePort = 32452,
	DummyCount = 100,
	PacketSizeMax = 1400,
	IsRecvDetailProc = false,
};

var DummyManager = new DummyManager();
DummyManager.Prepare();
await DummyManager.TestRepeatConnAsync(testUniqueIndex, config);

var testResult = DummyManager.GetTestResult(testUniqueIndex, config);

foreach (var report in testResult)
{
	Console.WriteLine(report.Detail);
	Console.WriteLine(report.Message);
}

DummyManager.EndTest();
