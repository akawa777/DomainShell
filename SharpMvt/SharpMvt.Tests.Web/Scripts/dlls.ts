                import * as _$_Handler_$_ from './post-handler'

                export namespace SharpMvt.Tests.Model.Utils {
                    export class Info {

                        TextList: string[]                        

                    }
                }

                export namespace SharpMvt.Tests.Model.Utils {
                    export class Message {

                        Text: string                        

                        Errors: string[]                        

                        InfoList: SharpMvt.Tests.Model.Utils.Info[]                        

                    }
                }

                export namespace System.Collections.Generic {
                    export class IEqualityComparer_1_SystemString {

                    }
                }

                export namespace SharpMvt.Tests.Model {
                    export class NoticeMessage {                        

                        constructor()

                        constructor(defaultMessage: number)

                        constructor(defaultMessage: string)

                        constructor(defaultMessage: SharpMvt.Tests.Model.Utils.Message)

                        constructor(defaultMessage: number, )

                        constructor(defaultMessage: number, param1: string)

                        constructor(defaultMessage: string, paramA: string, paramB: string, paramC: string)

                        constructor(defaultMessage? : any, paramA? : any, paramB? : any, paramC? : any) {                                                                        
                            if ((() => false)()) {
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]  === 'string')                                

                                && typeof arguments[1] !== 'undefined' && (typeof arguments[1]  === 'string')                                

                                && typeof arguments[2] !== 'undefined' && (typeof arguments[2]  === 'string')                                

                                && typeof arguments[3] !== 'undefined' && (typeof arguments[3]  === 'string')                                

                            ) {

                                this._$_constructorMetadataToken = 100663305;

                                this._$_constructorParameterValues.defaultMessage = arguments[0];

                                this._$_constructorParameterValues.paramA = arguments[1];

                                this._$_constructorParameterValues.paramB = arguments[2];

                                this._$_constructorParameterValues.paramC = arguments[3];

                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]  === 'number')                                

                                && typeof arguments[1] !== 'undefined' && (typeof arguments[1]  === 'string')                                

                            ) {

                                this._$_constructorMetadataToken = 100663304;

                                this._$_constructorParameterValues.defaultMessage = arguments[0];

                                this._$_constructorParameterValues.param1 = arguments[1];

                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]  === 'number')                                

                            ) {

                                this._$_constructorMetadataToken = 100663303;

                                this._$_constructorParameterValues.defaultMessage = arguments[0];

                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')                                

                            ) {

                                this._$_constructorMetadataToken = 100663307;

                                this._$_constructorParameterValues.defaultMessage = arguments[0];

                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]  === 'string')                                

                            ) {

                                this._$_constructorMetadataToken = 100663306;

                                this._$_constructorParameterValues.defaultMessage = arguments[0];

                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]  === 'number')                                

                            ) {

                                this._$_constructorMetadataToken = 100663301;

                                this._$_constructorParameterValues.defaultMessage = arguments[0];

                            }

                            else if (
                                true

                            ) {

                                this._$_constructorMetadataToken = 100663300;

                            }

                        }

                        private _$_constructorMetadataToken: number = 0;
                        private _$_constructorParameterValues: any = {};
                        private _$_validConstructorParams = true                        

                        Notice(): Promise<string>                        

                        Notice(message: string): Promise<string>                        

                        Notice(message: boolean): Promise<boolean>                        

                        Notice(message: number): Promise<string>                        

                        Notice(message: SharpMvt.Tests.Model.Utils.Message): Promise<string>                        

                        Notice(message: SharpMvt.Tests.Model.Utils.Message, param1: string): Promise<SharpMvt.Tests.Model.Utils.Message>                        

                        Notice(message: SharpMvt.Tests.Model.Utils.Message, paramA: string, paramB: string): Promise<boolean>                        

                        Notice(message: SharpMvt.Tests.Model.Utils.Message, paramA: string, paramB: string, paramC: string): Promise<string>                        

                        Notice(message? : any, paramA? : any, paramB? : any, paramC? : any): Promise<any> {                        
                            if (!this._$_validConstructorParams) {
                                throw new Error('invalid constructor arguments');
                            }

                            if ((() => false)()) {
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')                                

                                && typeof arguments[1] !== 'undefined' && (typeof arguments[1]  === 'string')                                

                                && typeof arguments[2] !== 'undefined' && (typeof arguments[2]  === 'string')                                

                                && typeof arguments[3] !== 'undefined' && (typeof arguments[3]  === 'string')                                

                            ) {
                                var _$_methodMetadataToken: number = 100663315;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.message = arguments[0];                                

                                _$_methodParameterValues.paramA = arguments[1];                                

                                _$_methodParameterValues.paramB = arguments[2];                                

                                _$_methodParameterValues.paramC = arguments[3];                                

                                return _$_Handler_$_.default.post<string>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model',
                                    className: 'SharpMvt.Tests.Model.NoticeMessage',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'Notice',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')                                

                                && typeof arguments[1] !== 'undefined' && (typeof arguments[1]  === 'string')                                

                                && typeof arguments[2] !== 'undefined' && (typeof arguments[2]  === 'string')                                

                            ) {
                                var _$_methodMetadataToken: number = 100663314;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.message = arguments[0];                                

                                _$_methodParameterValues.paramA = arguments[1];                                

                                _$_methodParameterValues.paramB = arguments[2];                                

                                return _$_Handler_$_.default.post<boolean>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model',
                                    className: 'SharpMvt.Tests.Model.NoticeMessage',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'Notice',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')                                

                                && typeof arguments[1] !== 'undefined' && (typeof arguments[1]  === 'string')                                

                            ) {
                                var _$_methodMetadataToken: number = 100663313;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.message = arguments[0];                                

                                _$_methodParameterValues.param1 = arguments[1];                                

                                return _$_Handler_$_.default.post<SharpMvt.Tests.Model.Utils.Message>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model',
                                    className: 'SharpMvt.Tests.Model.NoticeMessage',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'Notice',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')                                

                            ) {
                                var _$_methodMetadataToken: number = 100663312;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.message = arguments[0];                                

                                return _$_Handler_$_.default.post<string>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model',
                                    className: 'SharpMvt.Tests.Model.NoticeMessage',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'Notice',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]  === 'number')                                

                            ) {
                                var _$_methodMetadataToken: number = 100663311;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.message = arguments[0];                                

                                return _$_Handler_$_.default.post<string>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model',
                                    className: 'SharpMvt.Tests.Model.NoticeMessage',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'Notice',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]  === 'boolean')                                

                            ) {
                                var _$_methodMetadataToken: number = 100663310;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.message = arguments[0];                                

                                return _$_Handler_$_.default.post<boolean>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model',
                                    className: 'SharpMvt.Tests.Model.NoticeMessage',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'Notice',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]  === 'string')                                

                            ) {
                                var _$_methodMetadataToken: number = 100663309;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.message = arguments[0];                                

                                return _$_Handler_$_.default.post<string>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model',
                                    className: 'SharpMvt.Tests.Model.NoticeMessage',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'Notice',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else if (
                                true

                            ) {
                                var _$_methodMetadataToken: number = 100663308;
                                var _$_methodParameterValues: any = {};                                

                                return _$_Handler_$_.default.post<string>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model',
                                    className: 'SharpMvt.Tests.Model.NoticeMessage',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'Notice',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                        }

                        EchoNotice(message: SharpMvt.Tests.Model.Utils.Message): Promise<SharpMvt.Tests.Model.Utils.Message>                        

                        EchoNotice(message? : any): Promise<any> {                        
                            if (!this._$_validConstructorParams) {
                                throw new Error('invalid constructor arguments');
                            }

                            if ((() => false)()) {
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')                                

                            ) {
                                var _$_methodMetadataToken: number = 100663316;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.message = arguments[0];                                

                                return _$_Handler_$_.default.post<SharpMvt.Tests.Model.Utils.Message>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model',
                                    className: 'SharpMvt.Tests.Model.NoticeMessage',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'EchoNotice',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else {
                                throw new Error('invalid method arguments');
                            }

                        }

                        GetStream(stream: Blob): Promise<Blob>                        

                        GetStream(stream? : any): Promise<any> {                        
                            if (!this._$_validConstructorParams) {
                                throw new Error('invalid constructor arguments');
                            }

                            if ((() => false)()) {
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (arguments[0].constructor.name === 'Blob')                                

                            ) {
                                var _$_methodMetadataToken: number = 100663317;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.stream = arguments[0];                                

                                return _$_Handler_$_.default.post<Blob>({                                
                                    contentType: 'form',
                                    resultType: 'content',
                                    assemblyName: 'SharpMvt.Tests.Model',
                                    className: 'SharpMvt.Tests.Model.NoticeMessage',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'GetStream',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else {
                                throw new Error('invalid method arguments');
                            }

                        }

                        GetImage(stream: Blob): Promise<string>                        

                        GetImage(stream? : any): Promise<any> {                        
                            if (!this._$_validConstructorParams) {
                                throw new Error('invalid constructor arguments');
                            }

                            if ((() => false)()) {
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (arguments[0].constructor.name === 'Blob')                                

                            ) {
                                var _$_methodMetadataToken: number = 100663318;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.stream = arguments[0];                                

                                return _$_Handler_$_.default.post<string>({                                
                                    contentType: 'json',
                                    resultType: 'link',
                                    assemblyName: 'SharpMvt.Tests.Model',
                                    className: 'SharpMvt.Tests.Model.NoticeMessage',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'GetImage',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else {
                                throw new Error('invalid method arguments');
                            }

                        }

                        GetArray(array: string[]): Promise<SharpMvt.Tests.Model.Utils.Message[]>                        

                        GetArray(array: SharpMvt.Tests.Model.Utils.Message[]): Promise<SharpMvt.Tests.Model.Utils.Message[]>                        

                        GetArray(array? : any): Promise<any> {                        
                            if (!this._$_validConstructorParams) {
                                throw new Error('invalid constructor arguments');
                            }

                            if ((() => false)()) {
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (Array.isArray(arguments[0]))                                

                            ) {
                                var _$_methodMetadataToken: number = 100663320;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.array = arguments[0];                                

                                return _$_Handler_$_.default.post<SharpMvt.Tests.Model.Utils.Message[]>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model',
                                    className: 'SharpMvt.Tests.Model.NoticeMessage',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'GetArray',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (Array.isArray(arguments[0]))                                

                            ) {
                                var _$_methodMetadataToken: number = 100663319;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.array = arguments[0];                                

                                return _$_Handler_$_.default.post<SharpMvt.Tests.Model.Utils.Message[]>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model',
                                    className: 'SharpMvt.Tests.Model.NoticeMessage',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'GetArray',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else {
                                throw new Error('invalid method arguments');
                            }

                        }

                        GetList(): Promise<object>                        

                        GetList(array: SharpMvt.Tests.Model.Utils.Message[]): Promise<SharpMvt.Tests.Model.Utils.Message[]>                        

                        GetList(array? : any): Promise<any> {                        
                            if (!this._$_validConstructorParams) {
                                throw new Error('invalid constructor arguments');
                            }

                            if ((() => false)()) {
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (Array.isArray(arguments[0]))                                

                            ) {
                                var _$_methodMetadataToken: number = 100663321;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.array = arguments[0];                                

                                return _$_Handler_$_.default.post<SharpMvt.Tests.Model.Utils.Message[]>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model',
                                    className: 'SharpMvt.Tests.Model.NoticeMessage',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'GetList',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else if (
                                true

                            ) {
                                var _$_methodMetadataToken: number = 100663322;
                                var _$_methodParameterValues: any = {};                                

                                return _$_Handler_$_.default.post<object>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model',
                                    className: 'SharpMvt.Tests.Model.NoticeMessage',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'GetList',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                        }

                    }
                }

                export namespace SharpMvt.Tests.Model.Utils {
                    export class Echo {                        

                        constructor(init: boolean)

                        constructor(init? : any) {                                                                        
                            if ((() => false)()) {
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]  === 'boolean')                                

                            ) {

                                this._$_constructorMetadataToken = 100663297;

                                this._$_constructorParameterValues.init = arguments[0];

                            }

                            else {
                                this._$_validConstructorParams = false;
                            }

                        }

                        private _$_constructorMetadataToken: number = 0;
                        private _$_constructorParameterValues: any = {};
                        private _$_validConstructorParams = true                        

                        Do(message: SharpMvt.Tests.Model.Utils.Message): Promise<string>                        

                        Do(message: string): Promise<string>                        

                        Do(message: SharpMvt.Tests.Model.Utils.Message, can: boolean): Promise<SharpMvt.Tests.Model.Utils.Message>                        

                        Do(message? : any, can? : any): Promise<any> {                        
                            if (!this._$_validConstructorParams) {
                                throw new Error('invalid constructor arguments');
                            }

                            if ((() => false)()) {
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')                                

                                && typeof arguments[1] !== 'undefined' && (typeof arguments[1]  === 'boolean')                                

                            ) {
                                var _$_methodMetadataToken: number = 100663300;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.message = arguments[0];                                

                                _$_methodParameterValues.can = arguments[1];                                

                                return _$_Handler_$_.default.post<SharpMvt.Tests.Model.Utils.Message>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model.Utils',
                                    className: 'SharpMvt.Tests.Model.Utils.Echo',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'Do',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]  === 'string')                                

                            ) {
                                var _$_methodMetadataToken: number = 100663299;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.message = arguments[0];                                

                                return _$_Handler_$_.default.post<string>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model.Utils',
                                    className: 'SharpMvt.Tests.Model.Utils.Echo',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'Do',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else if (
                                true

                                && typeof arguments[0] !== 'undefined' && (typeof arguments[0]['Text'] !== 'undefined' && typeof arguments[0]['Errors'] !== 'undefined' && typeof arguments[0]['InfoList'] !== 'undefined')                                

                            ) {
                                var _$_methodMetadataToken: number = 100663298;
                                var _$_methodParameterValues: any = {};                                

                                _$_methodParameterValues.message = arguments[0];                                

                                return _$_Handler_$_.default.post<string>({                                
                                    contentType: 'json',
                                    resultType: 'json',
                                    assemblyName: 'SharpMvt.Tests.Model.Utils',
                                    className: 'SharpMvt.Tests.Model.Utils.Echo',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: 'Do',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }

                            else {
                                throw new Error('invalid method arguments');
                            }

                        }

                    }
                }

