define(["text!dashboard/templates/vmuschine.html?v=3", "Chart"], function (template, Chart) {
    var vmuschine = {
        template: template,
        data: {
            labels: ["January", "February", "March", "April", "May", "June", "July"],
            datasets: [
                {
                    label: "My First dataset",
                    fillColor: "rgba(220,220,220,0.5)",
                    strokeColor: "rgba(220,220,220,0.8)",
                    highlightFill: "rgba(220,220,220,0.75)",
                    highlightStroke: "rgba(220,220,220,1)",
                    data: [65, 59, 80, 81, 56, 55, 40]
                },
                {
                    label: "My Second dataset",
                    fillColor: "rgba(151,187,205,0.5)",
                    strokeColor: "rgba(151,187,205,0.8)",
                    highlightFill: "rgba(151,187,205,0.75)",
                    highlightStroke: "rgba(151,187,205,1)",
                    data: [28, 48, 40, 19, 86, 27, 90]
                }
            ]
        },
        canvas: function (el) {
            var self = this;

            self.Bar = function () {
                var ctx = el.getContext("2d");
                el.setAttribute("width", el.parentNode.clientWidth);
                el.setAttribute("height", 300);
                var myBarChart = new Chart(ctx).Bar(self.data);

                var timer = false;
                $(window).resize(function () {
                    if (timer !== false) {
                        clearTimeout(timer);
                    }
                    timer = setTimeout(function () {
                        myBarChart.destroy();                        
                        ctx = el.getContext("2d");
                        el.setAttribute("width", el.parentNode.clientWidth);
                        el.setAttribute("height", 300);
                        myBarChart = new Chart(ctx).Bar(self.data, {
                            animation: false
                        });
                    }, 200);
                });
            }
            
        },
        init: function (adapt, options) {
            var self = this;

            adapt(function () {
                self.Bar();
            });
        }
    }

    return vmuschine;
});