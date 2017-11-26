import html from './standardFrame.html'
import FrameVue, { DataContextGenericBase, FrameOptions, PanelOptions } from './frameVue'

class TwoColumnsFrameOptions extends FrameOptions {
    constructor() {
        super()
        this.headerPanel = new PanelOptions('headerPanel', 'header')
        this.headerPanel.classes.push({ 
            'col-sm-12': true
        })        
        this.leftPanel = new PanelOptions('leftPanel', 'main')
        this.leftPanel.classes.push({ 
            'col-sm-6': true,
            'auto-height': true
        })        
        this.rightPanel = new PanelOptions('rightPanel', 'main')
        this.rightPanel.classes.push({
            'col-sm-6': true,
            'auto-height': true
        })
        this.footerPanel = new PanelOptions('footerPanel', 'footer')
        this.footerPanel.classes.push({ 
            'col-sm-12': true
        })
    }

    headerPanel: PanelOptions    
    leftPanel: PanelOptions    
    rightPanel: PanelOptions
    footerPanel: PanelOptions

    getPanels(): PanelOptions[] {
        return [
            this.headerPanel,            
            this.leftPanel,            
            this.rightPanel,
            this.footerPanel
        ]
    }
}

export abstract class TwoColumnsDataContextBase extends DataContextGenericBase<TwoColumnsFrameOptions> {
    constructor() {
        super()
        this.frameOptions = new TwoColumnsFrameOptions()
    }
}

export default FrameVue.extend({        
    template: html
})
