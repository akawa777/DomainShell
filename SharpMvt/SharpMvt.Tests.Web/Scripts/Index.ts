import * as dlls from "./dlls"

(async () => {
    var utilBiz = new dlls.SharpMvt.Tests.Model.Utils.Echo(true);
    var biz = new dlls.SharpMvt.Tests.Model.NoticeMessage();
    
    var message = new dlls.SharpMvt.Tests.Model.Utils.Message();

    var rtn: string = await utilBiz.Do(message);
    var rtn2 = await biz.Notice(message);    
})();