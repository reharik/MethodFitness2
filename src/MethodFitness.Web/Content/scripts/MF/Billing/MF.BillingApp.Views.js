/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 11:24 AM
 * To change this template use File | Settings | File Templates.
 */

MF.Views.TrainerPaymentGridView = MF.Views.GridView.extend({
    viewLoaded:function(){
        MF.vent.bind("Redirect",this.showPayGrid,this);
    },
    showPayGrid:function(id){
        MF.vent.trigger("route","trainerpaymentlist/"+id);
    }
});