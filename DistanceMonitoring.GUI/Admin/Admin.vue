<template>
  <div>
    <h1 class="alert alert-dark">Панель администратора</h1>
<!-- 
    <table>
        <tr>
            <th>GUID</th>
            <th>Position</th>
            <th>Distance</th>
        </tr>
        <tr v-for="item in this.items_lst" :key="item.Guid">
            <td>{{item.Guid}}</td>
            <td>{{item.Position}}</td>
            <td>{{item.Distance}}</td>
        </tr>
    </table> -->

    <h3>Положение меток</h3>
    <div id="panel">

    </div>
  </div>
</template>

<script>
export default {
    data: function() {
        return {
            items_lst: [
                {
                    Guid: '0',
                    Position: [100, 100],
                    Distance: 0
                },
                {
                    Guid: '1',
                    Position: [200, 100],
                    Distance: 0
                },
                {
                    Guid: '2',
                    Position: [300, 300],
                    Distance: 0
                },
                {
                    Guid: '3',
                    Position: [700, 800],
                    Distance: 0
                }, 
            ],
        }
    },

    mounted: async function () {
        while(true) {

            let response = await fetch('/tags/', {
                method: 'GET',
                headers: {
                    "Content-Type": "text/json; charset=utf-8"
                }
            });
    
            this.data = await response.json();
            console.log(this.data);

            var myNode = document.getElementById("panel");
            while (myNode.firstChild) {
                myNode.removeChild(myNode.firstChild);
            }

            for (let item of await this.data.tags) {
                let newElem = document.createElement('div');
                    newElem.innerHTML = item.label;
        
                newElem.style.border = "1px solid blue";
                newElem.style.background = "yellow";
                newElem.style.position = "absolute";
                newElem.style.marginLeft = (item.position.x * 100) + 'px';
                newElem.style.marginTop = (item.position.y * 100)  + 'px';
                document.getElementById("panel").appendChild(newElem);
            }

            for (let item of await this.data.origins) {
                let newElem = document.createElement('div');
                    newElem.innerHTML = "Датчик";
        
                newElem.style.border = "1px solid blue";
                newElem.style.background = "blue";
                newElem.style.position = "absolute";
                newElem.style.color = "white";
                newElem.style.marginLeft = (item.x * 100) + 'px';
                newElem.style.marginTop = (item.y * 100)  + 'px';
                document.getElementById("panel").appendChild(newElem);
            }

        }



    },

    computed: {
        getData: async function() {
            let response = await fetch('/tags/', {
            method: 'GET',
            headers: {
                "Content-Type": "text/json; charset=utf-8"
            }
            });
            this.data = await response.json();
            console.log(this.data);
        }
    }
}

</script>

<style>
#panel {
    position: relative;
    display: block;
    width: 98%;
    height: 600px;
    overflow: scroll;
    margin: 1%;

    background: lightgray;
}

table {
    border: 1px solid grey;
    width: 100%;
}

th {
    border: 1px solid grey;
}
td {
    border: 1px solid grey;
}

h3 {
    margin: .75rem 1.25rem;
}
</style>
