import * as _$_Handler_$_ from './post-handler'
                                
                export namespace SharpMvt.Tests.Model {
                    export class BehaviorService {
                        constructor()
                        constructor()
                        constructor() {                            
                                                        
                        }
                        

                        static default() : BehaviorService {
                            var model = new BehaviorService();
                            

                            return model;
                        }
                    }
                }
            
                export namespace SharpMvt.Tests.Model.Utils {
                    export class Message {
                        constructor()
                        constructor(Text: string)
                        constructor(Text?: any) {                            
                            
                            if (Text !== undefined) {
                                this.Text = Text;
                            }
                                                
                        }
                        
                        Text: string
                    

                        static default() : Message {
                            var model = new Message();
                            
                            model.Text = '';
                    

                            return model;
                        }
                    }
                }
                 
                export namespace SharpMvt.Tests.Model {
                    export class NoticeMessage {
                        
                        constructor()
                
                        constructor(defaultMessage: number)
                
                        constructor()
                
                        constructor(defaultMessage: string)
                
                        constructor(defaultMessage: SharpMvt.Tests.Model.Utils.Message)
                
                        constructor(defaultMessage: number)
                
                        constructor(defaultMessage?: any) {
                            
                            if (
                                typeof arguments[0] !== 'undefined' && typeof arguments[0] == 'number') {
                                                                
                                this._$_constructorParameterValues.push(arguments[0]);                                
                                this._$_constructorParameterValues.push(null);                                    
                                
                                this._$_constructorParameterTypes.push('System.Int32');
                                this._$_constructorParameterTypes.push('SharpMvt.Tests.Model.BehaviorService');
                            }
                            else if (
                                typeof arguments[0] !== 'undefined' && arguments[0].constructor.name ==  'Message') {
                                                                
                                this._$_constructorParameterValues.push(arguments[0]);                                    
                                
                                this._$_constructorParameterTypes.push('SharpMvt.Tests.Model.Utils.Message');
                            }
                            else if (
                                typeof arguments[0] !== 'undefined' && typeof arguments[0] == 'string') {
                                                                
                                this._$_constructorParameterValues.push(arguments[0]);                                    
                                
                                this._$_constructorParameterTypes.push('System.String');
                            }
                            else if (arguments.length == 0) {
                                                                
                                this._$_constructorParameterValues.push(null);                                    
                                
                                this._$_constructorParameterTypes.push('SharpMvt.Tests.Model.BehaviorService');
                            }
                            else if (
                                typeof arguments[0] !== 'undefined' && typeof arguments[0] == 'number') {
                                                                
                                this._$_constructorParameterValues.push(arguments[0]);                                    
                                
                                this._$_constructorParameterTypes.push('System.Int32');
                            }
                        }

                        private _$_constructorParameterTypes: string[] = new Array<string>()
                        private _$_constructorParameterValues: any[] = new Array<any>()
                
                        
                        Notice(): Promise<string>
                
                        Notice(message: string): Promise<string>
                
                        Notice(message: number): Promise<string>
                
                        Notice(message: SharpMvt.Tests.Model.Utils.Message): Promise<string>
                
                        Notice(message?: any): Promise<string> {
                            var _$_methodParameterTypes: string[] = new Array<string>();
                            var _$_methodParameterValues: any[] = new Array<any>();                                                        

                            
                            if (
                                typeof arguments[0] !== 'undefined' && arguments[0].constructor.name == 'Message') {
                                
                                _$_methodParameterTypes.push('SharpMvt.Tests.Model.Utils.Message');                          
                            }
                            else if (
                                typeof arguments[0] !== 'undefined' && typeof arguments[0] == 'number') {
                                
                                _$_methodParameterTypes.push('System.Int32');                          
                            }
                            else if (
                                typeof arguments[0] !== 'undefined' && typeof arguments[0] == 'string') {
                                
                                _$_methodParameterTypes.push('System.String');                          
                            }                            

                            for (var key in arguments) {                                
                                _$_methodParameterValues.push(arguments[key]);                                
                            }

                            

                            return _$_Handler_$_.default.post<string>({
                                method: 'post',
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorParameterTypes: [].concat(this._$_constructorParameterTypes),
                                constructorParameterValues: [].concat(this._$_constructorParameterValues),
                                methodName: 'Notice',
                                methodParameterTypes: [].concat(_$_methodParameterTypes),
                                methodParameterValues: [].concat(_$_methodParameterValues)
                            } as any);                            
                        }
                
                        EchoNotice(message: SharpMvt.Tests.Model.Utils.Message): Promise<SharpMvt.Tests.Model.Utils.Message> {
                            var _$_methodParameterTypes: string[] = new Array<string>();
                            var _$_methodParameterValues: any[] = new Array<any>();                                                        

                            
                            if (
                                typeof arguments[0] !== 'undefined' && arguments[0].constructor.name == 'Message') {
                                
                                _$_methodParameterTypes.push('SharpMvt.Tests.Model.Utils.Message');                          
                            }                            

                            for (var key in arguments) {                                
                                _$_methodParameterValues.push(arguments[key]);                                
                            }

                            

                            return _$_Handler_$_.default.post<SharpMvt.Tests.Model.Utils.Message>({
                                method: 'post',
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model',
                                className: 'SharpMvt.Tests.Model.NoticeMessage',
                                constructorParameterTypes: [].concat(this._$_constructorParameterTypes),
                                constructorParameterValues: [].concat(this._$_constructorParameterValues),
                                methodName: 'EchoNotice',
                                methodParameterTypes: [].concat(_$_methodParameterTypes),
                                methodParameterValues: [].concat(_$_methodParameterValues)
                            } as any);                            
                        }
                
                    }
                }
            