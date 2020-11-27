<template>
  <div>
    <h1 class="alert alert-dark">Панель администратора</h1>

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
    </table>

    <h3>Положение датчиков</h3>
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
            ]
        }
    },

    mounted: function () {
        for (let item of this.items_lst) {

            let newElem = document.createElement('div');
            newElem.innerHTML = item.Guid;

            newElem.style.border = "1px solid blue";
            newElem.style.background = "yellow";
            newElem.style.position = "absolute";
            newElem.style.marginLeft = item.Position[0] + 'px';
            newElem.style.marginTop = item.Position[1]  + 'px';
            document.getElementById("panel").appendChild(newElem);
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
