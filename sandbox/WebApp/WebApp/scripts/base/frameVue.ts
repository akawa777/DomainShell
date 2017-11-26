import BaseVue from './baseVue'

export class PanelOptions {    
    constructor(key: string, type: string) {    
        this._key = key    
        this._type = type    
        this.classes = []
        this.styles = []
    }

    private _key:string
    private _type:string

    get key(): string {
        return this._key
    }

    get type(): string {
        return this._type
    }
    
    classes: { [key: string]: boolean; }[]
    styles: { [key: string]: any; }[]
}

export abstract class FrameOptions {
    abstract getPanels(): PanelOptions[]
}

abstract class DataContextBase {
    abstract getFrameOptions(): FrameOptions 
}

export abstract class DataContextGenericBase<TFrameOptions extends FrameOptions> extends DataContextBase {
    getFrameOptions(): FrameOptions {
        return this.frameOptions
    }

    frameOptions : TFrameOptions
}

export default BaseVue.extend({
    methods: {
        getPanelOptions: function(): PanelOptions[] {            
            var dataContext = this['dataContext'] as DataContextBase
            return dataContext.getFrameOptions().getPanels()
        },
        getPanelClassObject: function(key: string, panelOptions: PanelOptions[]) {
            var obj:{ [key: string]: boolean; } = {}
            
            panelOptions.forEach(x => {
                if (x.key != key) return

                x.classes.forEach(y =>{
                    obj = Object.assign(obj, y)
                })
            })

            return obj
        },
        getPanelStyleObject: function(key: string, panelOptions: PanelOptions[]) {
            var obj:{ [key: string]: boolean; } = {}
            
            panelOptions.forEach(x => {
                if (x.key != key) return

                x.styles.forEach(y =>{
                    obj = Object.assign(obj, y)
                })
            })

            return obj
        }
    }
})