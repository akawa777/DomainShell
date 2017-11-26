import BaseVue from '../base/baseVue'
import inputHtml from './input.html'

export default BaseVue.extend({
    template: inputHtml,
    data: function() {
        return { tempText: '' }
    },
    props: {
        text: String
    },
    methods: {
        blur: function() {            
            this.$emit('blur', this.tempText)
        }
    },
    watch: { 
        text: function() { 
            this.tempText = this.text
      }
    }
})
