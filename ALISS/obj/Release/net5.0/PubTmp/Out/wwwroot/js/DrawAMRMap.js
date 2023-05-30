   window.MoveCanvas = (divCanvas, divImage, x, y, divRegion,rad) => {
        var imgBigMap = document.getElementById(divImage);
        var cnvs = document.getElementById(divCanvas);
       var imgRegion = document.getElementById(divRegion);

        var scale = Math.min(cnvs.width / imgRegion.width, cnvs.height / imgRegion.height);

        cnvs.style.position = "absolute";
        cnvs.style.left = (imgBigMap.offsetLeft + x) + "px";
        cnvs.style.top = (imgBigMap.offsetTop + y) + "px";

        var ctx = cnvs.getContext("2d");
        ctx.clearRect(0, 0, ctx.width, ctx.height);
        ctx.stroke();
        ctx.drawImage(imgRegion, 0, 0, imgRegion.width * scale, imgRegion.height * scale);
}

window.MoveMaxMin = (divImageMap, divMinCanvas, divMaxCanvas, min_x, min_y, max_x, max_y, divRegionMin,divRegionMax) => {
    var imgMap = document.getElementById(divImageMap);

    var cnvsMin = document.getElementById(divMinCanvas);
    var imgRegionMin = document.getElementById(divRegionMin);
    var scaleMin = Math.min(cnvsMin.width / imgRegionMin.width, cnvsMin.height / imgRegionMin.height);

    cnvsMin.style.position = "absolute";
    cnvsMin.style.left = (imgMap.offsetLeft + min_x) + "px";
    cnvsMin.style.top = (imgMap.offsetTop + min_y) + "px";

    var cnvsMax = document.getElementById(divMaxCanvas);
    var imgRegionMax = document.getElementById(divRegionMax);
    var scaleMax = Math.min(cnvsMax.width / imgRegionMax.width, cnvsMax.height / imgRegionMax.height);

    cnvsMax.style.position = "absolute";
    cnvsMax.style.left = (imgMap.offsetLeft + max_x) + "px";
    cnvsMax.style.top = (imgMap.offsetTop + max_y) + "px";


    var ctxMin = cnvsMin.getContext("2d");
    ctxMin.clearRect(0, 0, ctxMin.width, ctxMin.height);
    ctxMin.stroke();
    ctxMin.drawImage(imgRegionMin, 0, 0, imgRegionMin.width * scaleMin, imgRegionMin.height * scaleMin);

    var ctxMax = cnvsMax.getContext("2d");
    ctxMax.clearRect(0, 0, ctxMax.width, ctxMax.height);
    ctxMax.stroke();
    ctxMax.drawImage(imgRegionMax, 0, 0, imgRegionMax.width * scaleMax, imgRegionMax.height * scaleMax);
}


window.showCoords = (divCanvas) => {
    var cnvs = document.getElementById(divCanvas);
    //var img = document.getElementById(divImage);

    //cnvs.style.position = "absolute";

    //cnvs.style.left = (img.offsetLeft + row1_x) + "px";
    //cnvs.style.top = (img.offsetTop - 11) + "px";  

    var ctx = cnvs.getContext("2d");
    ctx.beginPath();
    ctx.clearRect(0, 0, cnvs.width, cnvs.height);
    ctx.fillStyle = "red";
    ctx.fill();
    //ctx.fillStyle = "black";
    //ctx.fillRect(0, 0, cnvs.width, cnvs.height); //for white background
    //ctx.lineWidth = 2;
    //ctx.strokeStyle = "#FF0000";
    //ctx.strokeRect(0, 0, ctx.width, ctx.height);//for white background
    //window.alert("sometext :" + txtRegNo);
}


window.ClearCircle = (divCanvas) =>{  
    var cnvsCircle = document.getElementById(divCanvas);

    var ctx = cnvsCircle.getContext("2d");
    ctx.save();
    ctx.setTransform(1, 0, 0, 1, 0, 0);
    ctx.clearRect(0, 0, cnvsCircle.width, cnvsCircle.height);
    ctx.restore();
      
}

window.DrawCircle = (divCanvas, divImage, x, y, divRegion, radious, regNo) => {
    var imgBigMap = document.getElementById(divImage);
    var cnvs = document.getElementById(divCanvas);
    var imgRegion = document.getElementById(divRegion);

    var scale = Math.min(cnvs.width / imgRegion.width, cnvs.height / imgRegion.height);

    cnvs.style.position = "absolute";
    cnvs.style.left = (imgBigMap.offsetLeft + x) + "px";
    cnvs.style.top = (imgBigMap.offsetTop + y) + "px";

    var ctx = cnvs.getContext("2d");
    ctx.clearRect(0, 0, ctx.width, ctx.height);
    //ctx.drawImage(imgRegion, 0, 0, imgRegion.width * scale, imgRegion.height * scale);

    // Draw Circle
    ctx.beginPath();
    ctx.arc(imgRegion.width / 2, imgRegion.height / 2, radious, 0, Math.PI * 2, true);
    ctx.fillStyle = 'red';
    ctx.fill();
    ctx.lineWidth = 2;
    ctx.stroke();

    
    cnvs.addEventListener('click', function (event)
    {
        //alert('onclick');       
        //alert(regNo);
        var cnvs01 = document.getElementById('perc_1');
        var cnvs02 = document.getElementById('perc_2');
        var cnvs03 = document.getElementById('perc_3');
        var cnvs04 = document.getElementById('perc_4');
        var cnvs05 = document.getElementById('perc_5');
        var cnvs06 = document.getElementById('perc_6');
        var cnvs07 = document.getElementById('perc_7');
        var cnvs08 = document.getElementById('perc_8');
        var cnvs09 = document.getElementById('perc_9');
        var cnvs10 = document.getElementById('perc_10');
        var cnvs11 = document.getElementById('perc_11');
        var cnvs12 = document.getElementById('perc_12');
        var cnvs13 = document.getElementById('perc_13');     

        if (regNo == 1)
        {
            var innerHTML = cnvs01.innerHTML;
            var index = innerHTML.indexOf("highlight");
            if (index < 0) {
                innerHTML = "<span class='highlight'>" + innerHTML + "</span>";
                cnvs01.innerHTML = innerHTML;
            }
        }
        if (regNo == 2) {
            var innerHTML = cnvs02.innerHTML;
            var index = innerHTML.indexOf("highlight");
            if (index < 0) {
                innerHTML = "<span class='highlight'>" + innerHTML + "</span>";
                cnvs02.innerHTML = innerHTML;
            }
        }
        if (regNo == 3) {
            var innerHTML = cnvs03.innerHTML;
            var index = innerHTML.indexOf("highlight");
            if (index < 0) {
                innerHTML = "<span class='highlight'>" + innerHTML + "</span>";
                cnvs03.innerHTML = innerHTML;
            }
        }
        if (regNo == 4) {
            var innerHTML = cnvs04.innerHTML;
            var index = innerHTML.indexOf("highlight");
            if (index < 0) {
                innerHTML = "<span class='highlight'>" + innerHTML + "</span>";
                cnvs04.innerHTML = innerHTML;
            }
        }       
        if (regNo == 5) {
            var innerHTML = cnvs05.innerHTML;
            var index = innerHTML.indexOf("highlight");
            if (index < 0) {
                innerHTML = "<span class='highlight'>" + innerHTML + "</span>";
                cnvs05.innerHTML = innerHTML;
            }
        }   

        if (regNo != 1) {
            var innerHTML01 = cnvs01.innerHTML;
            innerHTML01 = innerHTML01.replace(/<\/?span[^>]*>/g, "");
            cnvs01.innerHTML = innerHTML01;
        }

        if (regNo != 2) {
            var innerHTML02 = cnvs02.innerHTML;
            innerHTML02 = innerHTML02.replace(/<\/?span[^>]*>/g, "");
            cnvs02.innerHTML = innerHTML02;
        }

        if (regNo != 3) {
            var innerHTML03 = cnvs03.innerHTML;
            innerHTML03 = innerHTML03.replace(/<\/?span[^>]*>/g, "");
            cnvs03.innerHTML = innerHTML03;
        }

        if (regNo != 4) {
            var innerHTML04 = cnvs04.innerHTML;
            innerHTML04 = innerHTML04.replace(/<\/?span[^>]*>/g, "");
            cnvs04.innerHTML = innerHTML04;
        }

        if (regNo != 5) {
            var innerHTML05 = cnvs05.innerHTML;
            innerHTML05 = innerHTML05.replace(/<\/?span[^>]*>/g, "");
            cnvs05.innerHTML = innerHTML05;
        }

        if (regNo != 6) {
            var innerHTML06 = cnvs06.innerHTML;
            innerHTML06 = innerHTML06.replace(/<\/?span[^>]*>/g, "");
            cnvs06.innerHTML = innerHTML06;
        }

        if (regNo != 7) {
            var innerHTML07 = cnvs07.innerHTML;
            innerHTML07 = innerHTML07.replace(/<\/?span[^>]*>/g, "");
            cnvs07.innerHTML = innerHTML07;
        }

        if (regNo != 8) {
            var innerHTML08 = cnvs08.innerHTML;
            innerHTML08 = innerHTML08.replace(/<\/?span[^>]*>/g, "");
            cnvs08.innerHTML = innerHTML08;
        }

        if (regNo != 9) {
            var innerHTML09 = cnvs09.innerHTML;
            innerHTML09 = innerHTML09.replace(/<\/?span[^>]*>/g, "");
            cnvs09.innerHTML = innerHTML09;
        }

        if (regNo != 10) {
            var innerHTML10 = cnvs10.innerHTML;
            innerHTML10 = innerHTML10.replace(/<\/?span[^>]*>/g, "");
            cnvs10.innerHTML = innerHTML10;
        }

        if (regNo != 11) {
            var innerHTML11 = cnvs11.innerHTML;
            innerHTML11 = innerHTML11.replace(/<\/?span[^>]*>/g, "");
            cnvs11.innerHTML = innerHTML11;
        }

        if (regNo != 12) {
            var innerHTML12 = cnvs12.innerHTML;
            innerHTML12 = innerHTML12.replace(/<\/?span[^>]*>/g, "");
            cnvs12.innerHTML = innerHTML12;
        }

        if (regNo != 13) {
            var innerHTML13 = cnvs13.innerHTML;
            innerHTML13 = innerHTML13.replace(/<\/?span[^>]*>/g, "");
            cnvs13.innerHTML = innerHTML13;
        }
       

    }, false);

}
	
	window.DrawValue = (divCanvas, divImage, x, y, txtValue) =>  {
        var cnvs = document.getElementById(divCanvas);
        var img = document.getElementById(divImage);

        cnvs.style.position = "absolute";
        cnvs.style.left = (img.offsetLeft + x) + "px";
        cnvs.style.top = (img.offsetTop + y) + "px";
         
        var ctx = cnvs.getContext("2d");
        ctx.beginPath();
        //ctx.clearRect(0, 0, cnvs.width, cnvs.height);
       
        ctx.font = "15px Arial";        
        ctx.fillText(txtValue,0 ,cnvs.height/2);
    }
	
    window.SetPositionCanvasInitial = (divImage,divCVS) => {       
        var img = document.getElementById(divImage);
        var cnvs01 = document.getElementById(divCVS[0]);
        var cnvs02 = document.getElementById(divCVS[1]);
        var cnvs03 = document.getElementById(divCVS[2]);
        var cnvs04 = document.getElementById(divCVS[3]);
        var cnvs05 = document.getElementById(divCVS[4]);
        var cnvs06 = document.getElementById(divCVS[5]);
        var cnvs07 = document.getElementById(divCVS[6]);
        var cnvs08 = document.getElementById(divCVS[7]);
        var cnvs09 = document.getElementById(divCVS[8]);
        var cnvs10 = document.getElementById(divCVS[9]);
        var cnvs11 = document.getElementById(divCVS[10]);
        var cnvs12 = document.getElementById(divCVS[11]);
        var cnvs13 = document.getElementById(divCVS[12]);

        cnvs01.style.position = "absolute";
        cnvs02.style.position = "absolute";
        cnvs03.style.position = "absolute";
        cnvs04.style.position = "absolute";
        cnvs05.style.position = "absolute";
        cnvs06.style.position = "absolute";
        cnvs07.style.position = "absolute";
        cnvs08.style.position = "absolute";
        cnvs09.style.position = "absolute";
        cnvs10.style.position = "absolute";
        cnvs11.style.position = "absolute";
        cnvs12.style.position = "absolute";
        cnvs13.style.position = "absolute";
          
        var fontvalue = "15px Arial";
        var fontcolor = "#999999";
        var row1_x = 41;
        var row2_x = 165;
        var txtValue = "0.0%";

        cnvs01.style.left = (img.offsetLeft + row1_x) + "px";         
        cnvs01.style.top = (img.offsetTop - 11) + "px";  

        cnvs02.style.left = (img.offsetLeft + row1_x) + "px";         
        cnvs02.style.top = (img.offsetTop + 32) + "px"; 

        cnvs03.style.left = (img.offsetLeft + row1_x) + "px";         
        cnvs03.style.top = (img.offsetTop + 77) + "px";  

        cnvs04.style.left = (img.offsetLeft + row1_x) + "px";         
        cnvs04.style.top = (img.offsetTop + 126) + "px";

        cnvs05.style.left = (img.offsetLeft + row1_x) + "px";         
        cnvs05.style.top = (img.offsetTop + 175) + "px";

        cnvs06.style.left = (img.offsetLeft + row1_x) + "px";         
        cnvs06.style.top = (img.offsetTop + 218) + "px";

        cnvs07.style.left = (img.offsetLeft + row1_x) + "px";         
        cnvs07.style.top = (img.offsetTop + 269) + "px";

        cnvs08.style.left = (img.offsetLeft + row2_x) + "px";         
        cnvs08.style.top = (img.offsetTop - 11) + "px";

        cnvs09.style.left = (img.offsetLeft + row2_x) + "px";         
        cnvs09.style.top = (img.offsetTop + 34) + "px";

        cnvs10.style.left = (img.offsetLeft + row2_x) + "px";         
        cnvs10.style.top = (img.offsetTop + 79) + "px";

        cnvs11.style.left = (img.offsetLeft + row2_x) + "px";         
        cnvs11.style.top = (img.offsetTop + 124) + "px";

        cnvs12.style.left = (img.offsetLeft + row2_x) + "px";         
        cnvs12.style.top = (img.offsetTop + 176) + "px";

        cnvs13.style.left = (img.offsetLeft + row2_x) + "px";         
        cnvs13.style.top = (img.offsetTop + 222) + "px";

        var ctx01 = cnvs01.getContext("2d");
        ctx01.beginPath();
        ctx01.font = fontvalue;   
        ctx01.fillStyle = fontcolor;
        ctx01.fillText(txtValue, 0, cnvs01.height / 2);

        var ctx02 = cnvs02.getContext("2d");
        ctx02.beginPath();
        ctx02.font = fontvalue;     
        ctx02.fillStyle = fontcolor;
        ctx02.fillText(txtValue, 0, cnvs02.height / 2);

        var ctx03 = cnvs03.getContext("2d");
        ctx03.beginPath();
        ctx03.font = fontvalue;        
        ctx03.fillStyle = fontcolor;
        ctx03.fillText(txtValue, 0, cnvs03.height / 2);

        var ctx04 = cnvs04.getContext("2d");
        ctx04.beginPath();
        ctx04.font = fontvalue;      
        ctx04.fillStyle = fontcolor;
        ctx04.fillText(txtValue, 0, cnvs04.height / 2);

        var ctx05 = cnvs05.getContext("2d");
        ctx05.beginPath();
        ctx05.font = fontvalue;        
        ctx05.fillStyle = fontcolor;
        ctx05.fillText(txtValue, 0, cnvs05.height / 2);

        var ctx06 = cnvs06.getContext("2d");
        ctx06.beginPath();
        ctx06.font = fontvalue;        
        ctx06.fillStyle = fontcolor;
        ctx06.fillText(txtValue, 0, cnvs06.height / 2);

        var ctx07 = cnvs07.getContext("2d");
        ctx07.beginPath();
        ctx07.font = fontvalue;  
        ctx07.fillStyle = fontcolor;
        ctx07.fillText(txtValue, 0, cnvs07.height / 2);

        var ctx08 = cnvs08.getContext("2d");
        ctx08.beginPath();
        ctx08.font = fontvalue;      
        ctx08.fillStyle = fontcolor;
        ctx08.fillText(txtValue, 0, cnvs08.height / 2);

        var ctx09 = cnvs09.getContext("2d");
        ctx09.beginPath();
        ctx09.font = fontvalue;  
        ctx09.fillStyle = fontcolor;
        ctx09.fillText(txtValue, 0, cnvs09.height / 2);

        var ctx10 = cnvs10.getContext("2d");
        ctx10.beginPath();
        ctx10.font = fontvalue;  
        ctx10.fillStyle = fontcolor;
        ctx10.fillText(txtValue, 0, cnvs10.height / 2);

        var ctx11 = cnvs11.getContext("2d");
        ctx11.beginPath();
        ctx11.font = fontvalue;        
        ctx11.fillStyle = fontcolor;
        ctx11.fillText(txtValue, 0, cnvs11.height / 2);

        var ctx12 = cnvs12.getContext("2d");
        ctx12.beginPath();
        ctx12.font = fontvalue;       
        ctx12.fillStyle = fontcolor;
        ctx12.fillText(txtValue, 0, cnvs12.height / 2);

        var ctx13 = cnvs13.getContext("2d");
        ctx13.beginPath();
        ctx13.font = fontvalue; 
        ctx13.fillStyle = fontcolor;
        ctx13.fillText(txtValue,0 ,cnvs13.height/2);

    }
	
    window.setPercentValue = (divcvs, txtValue) =>  {
        var cnvs01 = document.getElementById(divcvs[0]);
        var cnvs02 = document.getElementById(divcvs[1]);
        var cnvs03 = document.getElementById(divcvs[2]);
        var cnvs04 = document.getElementById(divcvs[3]);
        var cnvs05 = document.getElementById(divcvs[4]);
        var cnvs06 = document.getElementById(divcvs[5]);
        var cnvs07 = document.getElementById(divcvs[6]);
        var cnvs08 = document.getElementById(divcvs[7]);
        var cnvs09 = document.getElementById(divcvs[8]);
        var cnvs10 = document.getElementById(divcvs[9]);
        var cnvs11 = document.getElementById(divcvs[10]);
        var cnvs12 = document.getElementById(divcvs[11]);
        var cnvs13 = document.getElementById(divcvs[12]);

        var fontcolor = "#333333";
        var ctx01 = cnvs01.getContext("2d");
        var ctx02 = cnvs02.getContext("2d");
        var ctx03 = cnvs03.getContext("2d");
        var ctx04 = cnvs04.getContext("2d");
        var ctx05 = cnvs05.getContext("2d");
        var ctx06 = cnvs06.getContext("2d");
        var ctx07 = cnvs07.getContext("2d");
        var ctx08 = cnvs08.getContext("2d");
        var ctx09 = cnvs09.getContext("2d");
        var ctx10 = cnvs10.getContext("2d");
        var ctx11 = cnvs11.getContext("2d");
        var ctx12 = cnvs12.getContext("2d");
        var ctx13 = cnvs13.getContext("2d");

        ctx01.beginPath();     
        ctx01.clearRect(0, 0, cnvs01.width, cnvs01.height);
        ctx01.fillStyle = fontcolor;
        ctx01.fillText(txtValue[0] + "%", 0, cnvs01.height / 2);

        ctx02.beginPath();     
        ctx02.clearRect(0, 0, cnvs02.width, cnvs01.height);
        ctx02.fillStyle = fontcolor;
        ctx02.fillText(txtValue[1] + "%", 0, cnvs02.height / 2);

        ctx03.beginPath();     
        ctx03.clearRect(0, 0, cnvs03.width, cnvs01.height);
        ctx03.fillStyle = fontcolor;
        ctx03.fillText(txtValue[2] + "%", 0, cnvs03.height / 2);

        ctx04.beginPath();     
        ctx04.clearRect(0, 0, cnvs04.width, cnvs01.height);
        ctx04.fillStyle = fontcolor;
        ctx04.fillText(txtValue[3] + "%", 0, cnvs04.height / 2);

        ctx05.beginPath();     
        ctx05.clearRect(0, 0, cnvs05.width, cnvs01.height);
        ctx05.fillStyle = fontcolor;
        ctx05.fillText(txtValue[4] + "%", 0, cnvs05.height / 2);

        ctx06.beginPath();     
        ctx06.clearRect(0, 0, cnvs06.width, cnvs01.height);
        ctx06.fillStyle = fontcolor;
        ctx06.fillText(txtValue[5] + "%", 0, cnvs06.height / 2);

        ctx07.beginPath();     
        ctx07.clearRect(0, 0, cnvs07.width, cnvs01.height);
        ctx07.fillStyle = fontcolor;
        ctx07.fillText(txtValue[6] + "%", 0, cnvs07.height / 2);

        ctx08.beginPath();     
        ctx08.clearRect(0, 0, cnvs08.width, cnvs01.height);
        ctx08.fillStyle = fontcolor;
        ctx08.fillText(txtValue[7] + "%", 0, cnvs08.height / 2);

        ctx09.beginPath();     
        ctx09.clearRect(0, 0, cnvs09.width, cnvs01.height);
        ctx09.fillStyle = fontcolor;
        ctx09.fillText(txtValue[8] + "%", 0, cnvs09.height / 2);

        ctx10.beginPath();     
        ctx10.clearRect(0, 0, cnvs10.width, cnvs01.height);
        ctx10.fillStyle = fontcolor;
        ctx10.fillText(txtValue[9] + "%", 0, cnvs10.height / 2);

        ctx11.beginPath();     
        ctx11.clearRect(0, 0, cnvs11.width, cnvs01.height);
        ctx11.fillStyle = fontcolor;
        ctx11.fillText(txtValue[10] + "%", 0, cnvs11.height / 2);

        ctx12.beginPath();     
        ctx12.clearRect(0, 0, cnvs12.width, cnvs01.height);
        ctx12.fillStyle = fontcolor;
        ctx12.fillText(txtValue[11] + "%", 0, cnvs12.height / 2);

        ctx13.beginPath();     
        ctx13.clearRect(0, 0, cnvs13.width, cnvs01.height);
        ctx13.fillStyle = fontcolor;
        ctx13.fillText(txtValue[12] + "%", 0, cnvs13.height / 2);

}
