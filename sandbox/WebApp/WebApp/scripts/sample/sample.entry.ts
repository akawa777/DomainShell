import BaseVue, { Notification } from '../base/baseVue'
import TwoColumnsFrameVue, { TwoColumnsDataContextBase } from '../base/twoColumnsFrameVue'
import InputVue from '../base/inputVue'
import headerHtml from './header.html'
import listHtml from './list.html'
import detailHtml from './detail.html'
import footerHtml from './footer.html'
import $ from 'jquery'

(async function () {        
    class Item {    
        id:number
        name:string
    }    

    class DataContext extends TwoColumnsDataContextBase {
        constructor() {
            super()
            this.item = new Item()
            this.item.id = 0
            this.item.name = ''
            this.items = []

            this.frameOptions.leftPanel.classes.push({
                'col-sm-6': false,
                'col-sm-8': true
            })

            this.frameOptions.rightPanel.classes.push({
                'col-sm-6': false,
                'col-sm-4': true
            })

            this.frameOptions.rightPanel.styles.push({
                'color': 'red'
            })
        }

        item: Item
        items: Item[]
    }
    
    var headerPanel = BaseVue.extend({
        template: headerHtml,
        methods: {
            createNew: function() {
                var notification = new Notification(this, 'createNew')

                this.publishToParent(notification)    
            }
        }
    })    

    var listPanel = BaseVue.extend({
        template: listHtml,
        props: {
            dataContext: DataContext
        },
        methods: {
            edit: function(item: Item) {  
                var notification = new Notification(this, 'edit')
                notification.data = item

                this.publishToParent(notification)           
            }
        }
    })

    var detailPanel = BaseVue.extend({
        template: detailHtml,    
        components: {
            inputVue: InputVue
        },
        props: {
            dataContext: DataContext
        },
        methods: {
            blurName:function(name: string) {
                this.dataContext.item.name = name;
            }
        }
    })

    var footerPanel = BaseVue.extend({
        template : footerHtml,
        methods: {
            regist: function() {
                var notification = new Notification(this, 'regist')

                this.publishToParent(notification)    
            },
            remove: function() {
                var notification = new Notification(this, 'remove')

                this.publishToParent(notification)
            }
        }
    })

    new TwoColumnsFrameVue({        
        el: '#main',        
        components: {
            headerPanel: headerPanel,            
            leftPanel: listPanel,            
            rightPanel: detailPanel,
            footerPanel: footerPanel
        },
        data: {
            dataContext: new DataContext()
        },        
        mounted: function() {            
            var items: Item[] = []
            for (var i = 0; i < 10; i++) {
                var item = new Item()
                item.id = i + 1
                item.name = `name_${item.id}`
                items.push(item)
            }
            
            this.dataContext.items = items
        },
        methods: {            
            createNew: function() {                
                this.dataContext.item.id = 0
                this.dataContext.item.name = ''
            },
            edit: function(notification: Notification) {                  
                var item = notification.data as Item
                this.dataContext.item.id = item.id
                this.dataContext.item.name = item.name
            },
            regist: function() {      
                var item: Item
                if (this.dataContext.item.id == 0) {
                    item = new Item()
                    if (this.dataContext.items.length == 0) {
                        item.id = 1
                    } else {
                        item.id = this.dataContext.items[this.dataContext.items.length - 1].id + 1
                    }
                    item.name = this.dataContext.item.name
                    this.dataContext.items.push(item)
                    this.dataContext.item.id = item.id
                } else {
                    var itemObj  = this.dataContext.items.find(x => x.id == this.dataContext.item.id)
                    if (itemObj) {
                        item = itemObj
                        item.name = this.dataContext.item.name
                    }                
                }
            },
            remove: function() {                
                var index  = this.dataContext.items.findIndex(x => x.id == this.dataContext.item.id)
                this.dataContext.items.splice(index, 1)
                this.dataContext.item.id = 0
                this.dataContext.item.name = ''
            }
        }
    })
})()