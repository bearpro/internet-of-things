<template>
  <div>
    <h1 class="header">Панель администратора</h1>
    <h3 class="subheader">Положение меток</h3>
    <div class="page">
        <div>
            <div id="panel">
                <div
                    v-for="coord in this.coords.origins"
                    :key="coord.x + coord.y"
                    class="origin" :style="{ marginLeft: coord.x*100 + 'px', marginTop: coord.y*100 + 'px' }"
                    >
                        Датчик
                </div>
                
                <div 
                    v-for="tag in this.coords.tags" 
                    :key="tag.label"
                    class="target" :style="{ marginLeft: tag.position.x*1000 + 'px', marginTop: tag.position.y*1000 + 'px' }"
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
                    <tr>

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
            }
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
            this.coords = await response.json();
            setTimeout(this.make_fetch, 1000);
            
        }
    }
}

</script>

<style>
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
