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

const validUrl = (url) => {
	return (url || "").startsWith("/")
		? `http://qa.proxy.methodfit.net/csharp${url}`
		: url;
}

MF.repository= (function(){
	var repositoryCallback = async function(response, bodyType){
			if(response.type==="opaqueredirect"){
					window.location.replace('/signin');
					return null;
			}
			const result = bodyType === "text" ? await response.text() : await  response.json()

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
			const res = await fetch(validUrl(url), {
				method: "POST",
				body: data,
				credentials: 'include',
				redirect: 'manual'
			});
			return repositoryCallback(json);
	},
		ajaxGet:async function(url, data){
				throbber();
			
			const res = await fetch(validUrl(url) +"?"+ new URLSearchParams(data), {method:"GET", 
			headers: {
					"Content-Type": "text/html; charset=utf-8"
				},
				credentials: 'include',
				redirect: 'manual'
			})
			let bodyType = !res?.headers?.get("content-type")?.includes("json") ?"text":"";
			return repositoryCallback(res, bodyType );
		},
		ajaxPostModel: async function(url, data){
				throbber();
			const res = await fetch(validUrl(url), {
						method:"POST",
						body:data,
						headers: {
							'Content-Type': "application/json; charset=utf-8",
						},
						credentials: 'include',
						redirect: 'manual'
			})
			return repositoryCallback(res);
		}
	}
}());
