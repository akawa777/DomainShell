var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
define(["require", "exports", "./dlls"], function (require, exports, dlls) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    (() => __awaiter(this, void 0, void 0, function* () {
        var utilBiz = new dlls.SharpMvt.Tests.Model.Utils.Echo(true);
        var biz = new dlls.SharpMvt.Tests.Model.NoticeMessage();
        var message = new dlls.SharpMvt.Tests.Model.Utils.Message();
        var rtn = yield utilBiz.Do(message);
        var rtn2 = yield biz.Notice(message);
    }))();
});
//# sourceMappingURL=index.js.map