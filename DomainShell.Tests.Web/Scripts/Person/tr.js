define(["text!/views/person/tr"], function (template) {
    var tr = {
        node: template,
        validate: function() {
            var self = this;

            if (!self.$params || !self.$params.Id) {
                console.error("there are no Id in $params");

                return false;
            } else if (!self.$params || !self.$params.Name) {
                console.error("there are no Name in $params");

                return false;
            }

            return true;
        },
        ready: function () {
            var self = this;

            self.$context.pin("id", "a").attr("href", "/person/detail/" + self.$params.Id);
            self.$context.pin("id", "a").text(self.$params.Id);
            self.$context.pin("name").text(self.$params.Name);
        }
    }

    return tr;
});