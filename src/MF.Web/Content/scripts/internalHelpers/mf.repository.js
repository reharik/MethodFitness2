/**
 * Created by JetBrains RubyMine.
 * User: RHarik
 * Date: 10/13/11
 * Time: 10:52 AM
 * To change this template use File | Settings | File Templates.
 */
 if (typeof MF == "undefined") {
	var MF = {};
}

MF.repository= (function(){
	var repositoryCallback = function(result,callback){
			if(result.LoggedOut){
					window.location.replace(result.RedirectUrl);
					return null;
			}
			clearTimeout(MF.throbberTimeout);
			MF.showThrob=false;
			$("#ajaxLoading").hide();
			return result;
	};
	var throbber = function(){
			MF.showThrob=true;
			if(!MF.throbberTimeout){
					MF.throbberTimeout = setTimeout(function() {
							if(MF.showThrob) {
									$("#ajaxLoading").show();
							}
					}, 500);
			}
	};
	return {
		ajaxPost: async function(url, data){
			throbber();
			const res = await fetch(url, {
				method: "POST",
				body: data,
				credentials: 'include',
			});
			const json = await res.json()
			return repositoryCallback(json);
	},
		ajaxGet:async function(url, data){
				throbber();
			
			const res = await fetch(url +"?"+ new URLSearchParams(data), {method:"GET", 
			// headers: {
			// 		"Content-Type": "text/html; charset=utf-8"
			// 	},
				credentials: 'include',
			})
      let body;
      let temp = await res.text();
      try{
        body = JSON.parse(temp);
      }catch(err) {
        // no-op testing for json
      }
      if(!body) {			
        body = temp;
      }
			return repositoryCallback(body);
		},
		ajaxPostModel: async function(url, data){
				throbber();
			const res = await fetch(url, {
						method:"POST",
						body:data,
						headers: {
							'Content-Type': "application/json; charset=utf-8",
						},
						credentials: 'include',
			})
			const json = await  res.json()
			return repositoryCallback(json);
		}
	}
}());
