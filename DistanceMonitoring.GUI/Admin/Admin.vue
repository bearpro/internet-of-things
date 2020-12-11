<template>
  <div>
    <h1 class="header">Панель администратора</h1>
    <h3 class="subheader">Положение меток</h3>
    <div class="page">
        <div>
            <div id="panel">
                    <div
                        v-for="(coord, i) in this.coords.origins"
                        :key="i"
                        class="origin" :style="{ marginLeft: coord.x + 'px', marginTop: coord.y + 'px' }"
                        
                        >
                            Датчик
                    </div>
                    
                    <div 
                        v-for="tag in this.coords.tags" 
                        :key="tag.label"
                        class="target" :style="{ marginLeft: tag.position.x + 'px', marginTop: tag.position.y + 'px' }"
                        >
                            {{tag.label}}
                    </div>
            </div>
        </div>
        <div>
            <table class="statistics_table table">
                <thead> 
                    <tr>
                        <th>Участники коллизий</th>
                        <th>Время коллизий</th>
                    </tr>
                </thead>
                <tbody>
                    <tr
                        v-for="(s_tr, i) in statistics"
                        :key="i"
                        >
                        <td>{{s_tr.labels}}</td>
                        <td>{{s_tr.date}}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>       
  </div>
</template>

<script>
export default {
    data: function() {
        return {
            coords: {
                origins: []
            },
            statistics: []
        }
    },

    mounted: async function () {
        this.make_fetch();
    },

    methods: {
        make_fetch: async function() {
            let response = await fetch('/tags/', {
                method: 'GET',
                headers: {
                    "Content-Type": "text/json; charset=utf-8"
                }
            });
            let data = await response.json();
            console.log(data);
            this.coords = data;
            setTimeout(this.make_fetch, 50);
            
        }
    },

    watch: {
         coords: function() {
            if (this.coords.overlappingLabels.length != 0) {
                let l = this.coords.overlappingLabels.join(', ');
                let d = new Date();
                let options = {
                    era: 'long',
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric',
                    weekday: 'long',
                    timezone: 'UTC',
                    hour: 'numeric',
                    minute: 'numeric',
                    second: 'numeric'
                };
                this.statistics.push({
                    labels: l,
                    date: d.toLocaleString("ru", options)
                });
            }
        }
    }
}

</script>

<style>
.room {
    display: absolute;
    border: 3px solid black;
}

.statistics_table {
    border: 1px solid black;
}

.page {
    display: grid;
    grid-template-columns: 500px 1fr;
}

.origin {
    border: 1px solid black;
    background: blue;
    position: absolute;
    color: white;
}

.target {
    border: 1px solid black;
    background: yellow;
    position: absolute;
    color: black;
    border-radius: 50%;
}

.header {
    padding: 10px;
    background-color: lightblue;
}

.subheader {
    padding: 10px;

}

#panel {
    position: relative;
    display: block;
    width: 98%;
    height: 500px;
    overflow: scroll;
    margin: 1%;

    background: lightgray;
}
</style>
