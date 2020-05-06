namespace NPSBDummyLib
{
    public class ActionData
    {
        public TestCase TestCase { get; private set; }

        public string Text { get; private set; }

        public TestConfig Config { get; private set; }

        public ActionData(TestCase testCase, string text, TestConfig config)
        {
            TestCase = testCase;
            Text = text;
            Config = config;
            config.ActionCase = testCase;
        }

        public ActionData()
        {
        }
    }
}
