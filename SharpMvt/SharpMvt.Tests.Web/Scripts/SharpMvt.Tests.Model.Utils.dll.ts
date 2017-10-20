import * as _$_Handler_$_ from './post-handler'
                                
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
                 
                export namespace SharpMvt.Tests.Model.Utils {
                    export class Echo {
                        
                        constructor() {
                            
                        }

                        private _$_constructorParameterTypes: string[] = new Array<string>()
                        private _$_constructorParameterValues: any[] = new Array<any>()
                
                        
                        Do(message: SharpMvt.Tests.Model.Utils.Message): Promise<string> {
                            var _$_methodParameterTypes: string[] = new Array<string>();
                            var _$_methodParameterValues: any[] = new Array<any>();                                                        

                            
                            if (
                                typeof arguments[0] !== 'undefined' && arguments[0].constructor.name == 'Message') {
                                
                                _$_methodParameterTypes.push('SharpMvt.Tests.Model.Utils.Message');                          
                            }                            

                            for (var key in arguments) {                                
                                _$_methodParameterValues.push(arguments[key]);                                
                            }

                            

                            return _$_Handler_$_.default.post<string>({
                                method: 'post',
                                contentType: 'json',
                                resultType: 'json',
                                assemblyName: 'SharpMvt.Tests.Model.Utils',
                                className: 'SharpMvt.Tests.Model.Utils.Echo',
                                constructorParameterTypes: [].concat(this._$_constructorParameterTypes),
                                constructorParameterValues: [].concat(this._$_constructorParameterValues),
                                methodName: 'Do',
                                methodParameterTypes: [].concat(_$_methodParameterTypes),
                                methodParameterValues: [].concat(_$_methodParameterValues)
                            } as any);                            
                        }
                
                    }
                }
            