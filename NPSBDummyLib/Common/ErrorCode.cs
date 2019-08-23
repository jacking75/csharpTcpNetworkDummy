namespace NPSBDummyLib
{
    public enum EResultCode
    {
        RESULT_OK = 0,
        RESULT_RECV_ERROR = 1,         //< 받기 에러
        RESULT_CONNECTION_EXPIRED = 2, //< 커넥션 종료
        RESULT_FAILED_POP_CHANNEL = 3, //< 채널 데이터 가져오기 실패
    }
}
