define(["require", "exports", "./post-handler"], function (require, exports, _$_Handler_$_) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var SharpMvt;
    (function (SharpMvt) {
        var Tests;
        (function (Tests) {
            var Model;
            (function (Model) {
                var Utils;
                (function (Utils) {
                    class Info {
                    }
                    Utils.Info = Info;
                })(Utils = Model.Utils || (Model.Utils = {}));
            })(Model = Tests.Model || (Tests.Model = {}));
        })(Tests = SharpMvt.Tests || (SharpMvt.Tests = {}));
    })(SharpMvt = exports.SharpMvt || (exports.SharpMvt = {}));
    (function (SharpMvt) {
        var Tests;
        (function (Tests) {
            var Model;
            (function (Model) {
                var Utils;
                (function (Utils) {
                    class Message {
                    }
                    Utils.Message = Message;
                })(Utils = Model.Utils || (Model.Utils = {}));
            })(Model = Tests.Model || (Tests.Model = {}));
        })(Tests = SharpMvt.Tests || (SharpMvt.Tests = {}));
    })(SharpMvt = exports.SharpMvt || (exports.SharpMvt = {}));
    var System;
    (function (System) {
        var Collections;
        (function (Collections) {
            var Generic;
            (function (Generic) {
                class IEqualityComparer_1_SystemString {
                }
                Generic.IEqualityComparer_1_SystemString = IEqualityComparer_1_SystemString;
            })(Generic = Collections.Generic || (Collections.Generic = {}));
        })(Collections = System.Collections || (System.Collections = {}));
    })(System = exports.System || (exports.System = {}));
    (function (SharpMvt) {
        var Tests;
        (function (Tests) {
            var Model;
            (function (Model) {
                class NoticeMessage {
                    constructor(defaultMessage, paramA, paramB, paramC) {
                        this._$_constructorMetadataToken = 0;
                        this._$_constructorParameterValues = {};
                        this._$_validConstructorParams = true;
                        if ((() => false)()) {
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (typeof arguments[0] === 'string')
                            && typeof arguments[1] !== 'undefined' && (typeof arguments[1] === 'string')
                            && typeof arguments[2] !== 'undefined' && (typeof arguments[2] === 'string')
                            && typeof arguments[3] !== 'undefined' && (typeof arguments[3] === 'string')) {
                            this._$_constructorMetadataToken = 100663305;
                            this._$_constructorParameterValues.defaultMessage = arguments[0];
                            this._$_constructorParameterValues.paramA = arguments[1];
                            this._$_constructorParameterValues.paramB = arguments[2];
                            this._$_constructorParameterValues.paramC = arguments[3];
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (typeof arguments[0] === 'number')
                            && typeof arguments[1] !== 'undefined' && (typeof arguments[1] === 'string')) {
                            this._$_constructorMetadataToken = 100663304;
                            this._$_constructorParameterValues.defaultMessage = arguments[0];
                            this._$_constructorParameterValues.param1 = arguments[1];
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (typeof arguments[0] === 'number')) {
                            this._$_constructorMetadataToken = 100663303;
                            this._$_constructorParameterValues.defaultMessage = arguments[0];
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')) {
                            this._$_constructorMetadataToken = 100663307;
                            this._$_constructorParameterValues.defaultMessage = arguments[0];
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (typeof arguments[0] === 'string')) {
                            this._$_constructorMetadataToken = 100663306;
                            this._$_constructorParameterValues.defaultMessage = arguments[0];
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (typeof arguments[0] === 'number')) {
                            this._$_constructorMetadataToken = 100663301;
                            this._$_constructorParameterValues.defaultMessage = arguments[0];
                        }
                        else if (true) {
                            this._$_constructorMetadataToken = 100663300;
                        }
                    }
                    Notice(message, paramA, paramB, paramC) {
                        if (!this._$_validConstructorParams) {
                            throw new Error('invalid constructor arguments');
                        }
                        if ((() => false)()) {
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')
                            && typeof arguments[1] !== 'undefined' && (typeof arguments[1] === 'string')
                            && typeof arguments[2] !== 'undefined' && (typeof arguments[2] === 'string')
                            && typeof arguments[3] !== 'undefined' && (typeof arguments[3] === 'string')) {
                            var _$_methodMetadataToken = 100663315;
                            var _$_methodParameterValues = {};
                            _$_methodParameterValues.message = arguments[0];
                            _$_methodParameterValues.paramA = arguments[1];
                            _$_methodParameterValues.paramB = arguments[2];
                            _$_methodParameterValues.paramC = arguments[3];
                            return _$_Handler_$_.default.post({
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorMetadataToken: this._$_constructorMetadataToken,
                                constructorParameterValues: this._$_constructorParameterValues,
                                methodName: 'Notice',
                                methodMetadataToken: _$_methodMetadataToken,
                                methodParameterValues: _$_methodParameterValues
                            });
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')
                            && typeof arguments[1] !== 'undefined' && (typeof arguments[1] === 'string')
                            && typeof arguments[2] !== 'undefined' && (typeof arguments[2] === 'string')) {
                            var _$_methodMetadataToken = 100663314;
                            var _$_methodParameterValues = {};
                            _$_methodParameterValues.message = arguments[0];
                            _$_methodParameterValues.paramA = arguments[1];
                            _$_methodParameterValues.paramB = arguments[2];
                            return _$_Handler_$_.default.post({
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorMetadataToken: this._$_constructorMetadataToken,
                                constructorParameterValues: this._$_constructorParameterValues,
                                methodName: 'Notice',
                                methodMetadataToken: _$_methodMetadataToken,
                                methodParameterValues: _$_methodParameterValues
                            });
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')
                            && typeof arguments[1] !== 'undefined' && (typeof arguments[1] === 'string')) {
                            var _$_methodMetadataToken = 100663313;
                            var _$_methodParameterValues = {};
                            _$_methodParameterValues.message = arguments[0];
                            _$_methodParameterValues.param1 = arguments[1];
                            return _$_Handler_$_.default.post({
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorMetadataToken: this._$_constructorMetadataToken,
                                constructorParameterValues: this._$_constructorParameterValues,
                                methodName: 'Notice',
                                methodMetadataToken: _$_methodMetadataToken,
                                methodParameterValues: _$_methodParameterValues
                            });
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')) {
                            var _$_methodMetadataToken = 100663312;
                            var _$_methodParameterValues = {};
                            _$_methodParameterValues.message = arguments[0];
                            return _$_Handler_$_.default.post({
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorMetadataToken: this._$_constructorMetadataToken,
                                constructorParameterValues: this._$_constructorParameterValues,
                                methodName: 'Notice',
                                methodMetadataToken: _$_methodMetadataToken,
                                methodParameterValues: _$_methodParameterValues
                            });
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (typeof arguments[0] === 'number')) {
                            var _$_methodMetadataToken = 100663311;
                            var _$_methodParameterValues = {};
                            _$_methodParameterValues.message = arguments[0];
                            return _$_Handler_$_.default.post({
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorMetadataToken: this._$_constructorMetadataToken,
                                constructorParameterValues: this._$_constructorParameterValues,
                                methodName: 'Notice',
                                methodMetadataToken: _$_methodMetadataToken,
                                methodParameterValues: _$_methodParameterValues
                            });
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (typeof arguments[0] === 'boolean')) {
                            var _$_methodMetadataToken = 100663310;
                            var _$_methodParameterValues = {};
                            _$_methodParameterValues.message = arguments[0];
                            return _$_Handler_$_.default.post({
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorMetadataToken: this._$_constructorMetadataToken,
                                constructorParameterValues: this._$_constructorParameterValues,
                                methodName: 'Notice',
                                methodMetadataToken: _$_methodMetadataToken,
                                methodParameterValues: _$_methodParameterValues
                            });
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (typeof arguments[0] === 'string')) {
                            var _$_methodMetadataToken = 100663309;
                            var _$_methodParameterValues = {};
                            _$_methodParameterValues.message = arguments[0];
                            return _$_Handler_$_.default.post({
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorMetadataToken: this._$_constructorMetadataToken,
                                constructorParameterValues: this._$_constructorParameterValues,
                                methodName: 'Notice',
                                methodMetadataToken: _$_methodMetadataToken,
                                methodParameterValues: _$_methodParameterValues
                            });
                        }
                        else if (true) {
                            var _$_methodMetadataToken = 100663308;
                            var _$_methodParameterValues = {};
                            return _$_Handler_$_.default.post({
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorMetadataToken: this._$_constructorMetadataToken,
                                constructorParameterValues: this._$_constructorParameterValues,
                                methodName: 'Notice',
                                methodMetadataToken: _$_methodMetadataToken,
                                methodParameterValues: _$_methodParameterValues
                            });
                        }
                    }
                    EchoNotice(message) {
                        if (!this._$_validConstructorParams) {
                            throw new Error('invalid constructor arguments');
                        }
                        if ((() => false)()) {
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')) {
                            var _$_methodMetadataToken = 100663316;
                            var _$_methodParameterValues = {};
                            _$_methodParameterValues.message = arguments[0];
                            return _$_Handler_$_.default.post({
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorMetadataToken: this._$_constructorMetadataToken,
                                constructorParameterValues: this._$_constructorParameterValues,
                                methodName: 'EchoNotice',
                                methodMetadataToken: _$_methodMetadataToken,
                                methodParameterValues: _$_methodParameterValues
                            });
                        }
                        else {
                            throw new Error('invalid method arguments');
                        }
                    }
                    GetStream(stream) {
                        if (!this._$_validConstructorParams) {
                            throw new Error('invalid constructor arguments');
                        }
                        if ((() => false)()) {
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (arguments[0].constructor.name === 'Blob')) {
                            var _$_methodMetadataToken = 100663317;
                            var _$_methodParameterValues = {};
                            _$_methodParameterValues.stream = arguments[0];
                            return _$_Handler_$_.default.post({
                                contentType: 'form',
                                resultType: 'content',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorMetadataToken: this._$_constructorMetadataToken,
                                constructorParameterValues: this._$_constructorParameterValues,
                                methodName: 'GetStream',
                                methodMetadataToken: _$_methodMetadataToken,
                                methodParameterValues: _$_methodParameterValues
                            });
                        }
                        else {
                            throw new Error('invalid method arguments');
                        }
                    }
                    GetImage(stream) {
                        if (!this._$_validConstructorParams) {
                            throw new Error('invalid constructor arguments');
                        }
                        if ((() => false)()) {
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (arguments[0].constructor.name === 'Blob')) {
                            var _$_methodMetadataToken = 100663318;
                            var _$_methodParameterValues = {};
                            _$_methodParameterValues.stream = arguments[0];
                            return _$_Handler_$_.default.post({
                                contentType: 'json',
                                resultType: 'link',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorMetadataToken: this._$_constructorMetadataToken,
                                constructorParameterValues: this._$_constructorParameterValues,
                                methodName: 'GetImage',
                                methodMetadataToken: _$_methodMetadataToken,
                                methodParameterValues: _$_methodParameterValues
                            });
                        }
                        else {
                            throw new Error('invalid method arguments');
                        }
                    }
                    GetArray(array) {
                        if (!this._$_validConstructorParams) {
                            throw new Error('invalid constructor arguments');
                        }
                        if ((() => false)()) {
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (Array.isArray(arguments[0]))) {
                            var _$_methodMetadataToken = 100663320;
                            var _$_methodParameterValues = {};
                            _$_methodParameterValues.array = arguments[0];
                            return _$_Handler_$_.default.post({
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorMetadataToken: this._$_constructorMetadataToken,
                                constructorParameterValues: this._$_constructorParameterValues,
                                methodName: 'GetArray',
                                methodMetadataToken: _$_methodMetadataToken,
                                methodParameterValues: _$_methodParameterValues
                            });
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (Array.isArray(arguments[0]))) {
                            var _$_methodMetadataToken = 100663319;
                            var _$_methodParameterValues = {};
                            _$_methodParameterValues.array = arguments[0];
                            return _$_Handler_$_.default.post({
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorMetadataToken: this._$_constructorMetadataToken,
                                constructorParameterValues: this._$_constructorParameterValues,
                                methodName: 'GetArray',
                                methodMetadataToken: _$_methodMetadataToken,
                                methodParameterValues: _$_methodParameterValues
                            });
                        }
                        else {
                            throw new Error('invalid method arguments');
                        }
                    }
                    GetList(array) {
                        if (!this._$_validConstructorParams) {
                            throw new Error('invalid constructor arguments');
                        }
                        if ((() => false)()) {
                        }
                        else if (true
                            && typeof arguments[0] !== 'undefined' && (Array.isArray(arguments[0]))) {
                            var _$_methodMetadataToken = 100663321;
                            var _$_methodParameterValues = {};
                            _$_methodParameterValues.array = arguments[0];
                            return _$_Handler_$_.default.post({
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorMetadataToken: this._$_constructorMetadataToken,
                                constructorParameterValues: this._$_constructorParameterValues,
                                methodName: 'GetList',
                                methodMetadataToken: _$_methodMetadataToken,
                                methodParameterValues: _$_methodParameterValues
                            });
                        }
                        else if (true) {
                            var _$_methodMetadataToken = 100663322;
                            var _$_methodParameterValues = {};
                            return _$_Handler_$_.default.post({
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorMetadataToken: this._$_constructorMetadataToken,
                                constructorParameterValues: this._$_constructorParameterValues,
                                methodName: 'GetList',
                                methodMetadataToken: _$_methodMetadataToken,
                                methodParameterValues: _$_methodParameterValues
                            });
                        }
                    }
                }
                Model.NoticeMessage = NoticeMessage;
            })(Model = Tests.Model || (Tests.Model = {}));
        })(Tests = SharpMvt.Tests || (SharpMvt.Tests = {}));
    })(SharpMvt = exports.SharpMvt || (exports.SharpMvt = {}));
    (function (SharpMvt) {
        var Tests;
        (function (Tests) {
            var Model;
            (function (Model) {
                var Utils;
                (function (Utils) {
                    class Echo {
                        constructor(init) {
                            this._$_constructorMetadataToken = 0;
                            this._$_constructorParameterValues = {};
                            this._$_validConstructorParams = true;
                            if ((() => false)()) {
                            }
                            else if (true
                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0] === 'boolean')) {
                                this._$_constructorMetadataToken = 100663297;
                                this._$_constructorParameterValues.init = arguments[0];
                            }
                            else {
                                this._$_validConstructorParams = false;
                            }
                        }
                        Do(message, can) {
                            if (!this._$_validConstructorParams) {
                                throw new Error('invalid constructor arguments');
                            }
                            if ((() => false)()) {
                            }
                            else if (true
                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')
                                && typeof arguments[1] !== 'undefined' && (typeof arguments[1] === 'boolean')) {
                                var _$_methodMetadataToken = 100663300;
                                var _$_methodParameterValues = {};
                                _$_methodParameterValues.message = arguments[0];
                                _$_methodParameterValues.can = arguments[1];
                                return _$_Handler_$_.default.post({
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model.Utils',
                                    className: 'SharpMvt.Tests.Model.Utils.Echo',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'Do',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                });
                            }
                            else if (true
                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0] === 'string')) {
                                var _$_methodMetadataToken = 100663299;
                                var _$_methodParameterValues = {};
                                _$_methodParameterValues.message = arguments[0];
                                return _$_Handler_$_.default.post({
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model.Utils',
                                    className: 'SharpMvt.Tests.Model.Utils.Echo',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'Do',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                });
                            }
                            else if (true
                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')) {
                                var _$_methodMetadataToken = 100663298;
                                var _$_methodParameterValues = {};
                                _$_methodParameterValues.message = arguments[0];
                                return _$_Handler_$_.default.post({
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model.Utils',
                                    className: 'SharpMvt.Tests.Model.Utils.Echo',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'Do',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                });
                            }
                            else {
                                throw new Error('invalid method arguments');
                            }
                        }
                    }
                    Utils.Echo = Echo;
                })(Utils = Model.Utils || (Model.Utils = {}));
            })(Model = Tests.Model || (Tests.Model = {}));
        })(Tests = SharpMvt.Tests || (SharpMvt.Tests = {}));
    })(SharpMvt = exports.SharpMvt || (exports.SharpMvt = {}));
});
//# sourceMappingURL=dlls.js.map