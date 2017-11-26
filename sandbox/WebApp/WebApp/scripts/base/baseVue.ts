import Vue from 'vue'

export class Notification {
    constructor (selfVue: Vue, name: string) {
        if (!selfVue.$vnode.componentOptions || selfVue.$vnode.componentOptions.tag) {
            this.tag = ''
        } else {
            this.tag = selfVue.$vnode.componentOptions.tag
        }
        
        this.name = name
    }

    tag?: string
    name: string
    data: any
    event: Event
}

export default Vue.extend({        
    methods: {        
        publishToParent: function(notification: Notification) {
            if (!notification || notification == null) {
                throw new Error('notification is undefined or null.')
            }
            if (notification.name === undefined || notification.name == null || notification.name == '') {
                throw new Error('notification.name is required.')
            }

            this.$emit('publishToParent', notification)        
        },
        handleFromChild: function(notification: Notification) {  
            if (!this[notification.name] || typeof this[notification.name] !==  'function') {
                throw new Error(`function` + notification.name + ` is not defined.`)
            }

            this[notification.name](notification)
        },
        hasComponent: function(componentKey: string) {
            return this.$options.components && this.$options.components[componentKey]
        }
    }
})