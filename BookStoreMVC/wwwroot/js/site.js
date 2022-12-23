// filter = url => new Promise((accept, reject) => $.ajax({
//     url: url,
//     dataType: "JSON",
//     method: "GET",
//     success: data => accept(data),
// }));

// $.each(Array, async function (i, obj){
//     $("#bookList").append(``)
//    
//     let res = await filter
// })
spinner = () => {
    const spinner = `<div id="spinner" role="status">
                        <svg class="inline mr-2 w-8 h-8 text-gray-200 animate-spin dark:text-gray-600 fill-green-500" viewBox="0 0 100 101" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z" fill="currentColor"/>
                            <path d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z" fill="currentFill"/>
                        </svg>
                        <span class="sr-only">Loading...</span>
                     </div>`;
    
    $("#bookList").html(spinner);
}


filterBooks = (url, filterBy, filterValue) => {
    $("#bookList").empty();
    
    spinner();
    
    $.getJSON(url+`/books/api?filterBy=${filterBy}&filterValue=${filterValue}`, function (data){
        $("#spinner").hide();
        
        
        
        $.each(data, function (key, obj){
            $("#bookList").append(`<div class=" w-[460px] h-[280px]  relative flex hover:cursor-pointer">
                    <img class="object-cover w-[218px] h-[283px] rounded-lg z-20 " src=${Object.byString(obj, 'book.signedUrl')} alt="">
                    <a href="${url}/book/${Object.byString(obj, 'book.id')}/${Object.byString(obj, 'book.title')}">
                        <div class=" absolute inset-y-0 right-0  w-[300px]  flex justify-center items-center">
                            <div
                                class="w-[300px] h-[230px] p-5 pl-[90px] bg-white hover:shadow-lg transition-shadow duration-200 rounded-lg flex flex-col ">
                                <h1 class="text-lg  font-bold text-titleText truncate">${Object.byString(obj, 'book.title')}</h1>
                                <h2 class="font-light text-xs text-normalText mb-1 ">
                                    ${Object.byString(obj, 'book.author.displayName')}
                                </h2>
<!--                                <h2 class="font-light text-sm text-primary flex items-center  ">-->
<!--                                    @for (int i = 0; i < product.Rating; i++)-->
<!--                                    {-->
<!--                                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"-->
<!--                                             stroke="currentColor" class="w-5 h-5 fill-primary">-->
<!--                                            <path stroke-linecap="round" stroke-linejoin="round"-->
<!--                                                  d="M11.48 3.499a.562.562 0 011.04 0l2.125 5.111a.563.563 0 00.475.345l5.518.442c.499.04.701.663.321.988l-4.204 3.602a.563.563 0 00-.182.557l1.285 5.385a.562.562 0 01-.84.61l-4.725-2.885a.563.563 0 00-.586 0L6.982 20.54a.562.562 0 01-.84-.61l1.285-5.386a.562.562 0 00-.182-.557l-4.204-3.602a.563.563 0 01.321-.988l5.518-.442a.563.563 0 00.475-.345L11.48 3.5z"/>-->
<!--                                        </svg>-->
<!--                                    }-->
<!--                                    @for (int i = product.Rating; i < 5; i++)-->
<!--                                    {-->
<!--                                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"-->
<!--                                             stroke="currentColor" class="w-5 h-5">-->
<!--                                            <path stroke-linecap="round" stroke-linejoin="round"-->
<!--                                                  d="M11.48 3.499a.562.562 0 011.04 0l2.125 5.111a.563.563 0 00.475.345l5.518.442c.499.04.701.663.321.988l-4.204 3.602a.563.563 0 00-.182.557l1.285 5.385a.562.562 0 01-.84.61l-4.725-2.885a.563.563 0 00-.586 0L6.982 20.54a.562.562 0 01-.84-.61l1.285-5.386a.562.562 0 00-.182-.557l-4.204-3.602a.563.563 0 01.321-.988l5.518-.442a.563.563 0 00.475-.345L11.48 3.5z"/>-->
<!--                                        </svg>-->
<!--                                    }-->

<!--                                </h2>-->
                                    <h2 class="font-light text-sm text-primary flex items-center" id="rating-${key}">
                                    </h2>
                                <h3 class="text-sm line-clamp-2 min-h-[3rem]">${Object.byString(obj, 'book.description')}</h3>
                                <h2 class="font-bold text-primary mt-1 mb-2">${new Intl.NumberFormat('he-HE', { style: 'currency', currency: 'VND' ,maximumFractionDigits: 2 }).format(Object.byString(obj, 'price.hardcover'))}</h2>
                                <form class="inline-block bgButton py-1" method="post" action="/cart/add?id=${Object.byString(obj, 'id')}">
                                    <button class="" type="submit">Add to cart</button>
                                </form>
                            </div>
                        </div>
                    </a>`)
            // for (let i = 0; i < Object.byString(obj, 'rating'); i++) {
            //     $(`#rating-${key}`).append(`<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-5 h-5 fill-primary">
            //                                     <path stroke-linecap="round" stroke-linejoin="round"
            //                                     d="M11.48 3.499a.562.562 0 011.04 0l2.125 5.111a.563.563 0 00.475.345l5.518.442c.499.04.701.663.321.988l-4.204 3.602a.563.563 0 00-.182.557l1.285 5.385a.562.562 0 01-.84.61l-4.725-2.885a.563.563 0 00-.586 0L6.982 20.54a.562.562 0 01-.84-.61l1.285-5.386a.562.562 0 00-.182-.557l-4.204-3.602a.563.563 0 01.321-.988l5.518-.442a.563.563 0 00.475-.345L11.48 3.5z"/>
            //                                 </svg>`);
            // }
            // for (let i = Object.byString(obj, 'rating'); i < 5; i++) {
            //     $(`#rating-${key}`).append(`<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
            //                                  stroke="currentColor" class="w-5 h-5">
            //                                     <path stroke-linecap="round" stroke-linejoin="round"
            //                                     d="M11.48 3.499a.562.562 0 011.04 0l2.125 5.111a.563.563 0 00.475.345l5.518.442c.499.04.701.663.321.988l-4.204 3.602a.563.563 0 00-.182.557l1.285 5.385a.562.562 0 01-.84.61l-4.725-2.885a.563.563 0 00-.586 0L6.982 20.54a.562.562 0 01-.84-.61l1.285-5.386a.562.562 0 00-.182-.557l-4.204-3.602a.563.563 0 01.321-.988l5.518-.442a.563.563 0 00.475-.345L11.48 3.5z"/>
            //                                 </svg>`);
            for (let i = 0; i < 4; i++) {
                if (i <= Object.byString(obj, 'rating') && Object.byString(obj, 'rating') > 0) {
                    $(`#rating-${key}`).append(`<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-5 h-5 fill-primary">
                                                <path stroke-linecap="round" stroke-linejoin="round"
                                                d="M11.48 3.499a.562.562 0 011.04 0l2.125 5.111a.563.563 0 00.475.345l5.518.442c.499.04.701.663.321.988l-4.204 3.602a.563.563 0 00-.182.557l1.285 5.385a.562.562 0 01-.84.61l-4.725-2.885a.563.563 0 00-.586 0L6.982 20.54a.562.562 0 01-.84-.61l1.285-5.386a.562.562 0 00-.182-.557l-4.204-3.602a.563.563 0 01.321-.988l5.518-.442a.563.563 0 00.475-.345L11.48 3.5z"/>
                                            </svg>`);
                } else {
                    $(`#rating-${key}`).append(`<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
                                             stroke="currentColor" class="w-5 h-5">
                                                <path stroke-linecap="round" stroke-linejoin="round"
                                                d="M11.48 3.499a.562.562 0 011.04 0l2.125 5.111a.563.563 0 00.475.345l5.518.442c.499.04.701.663.321.988l-4.204 3.602a.563.563 0 00-.182.557l1.285 5.385a.562.562 0 01-.84.61l-4.725-2.885a.563.563 0 00-.586 0L6.982 20.54a.562.562 0 01-.84-.61l1.285-5.386a.562.562 0 00-.182-.557l-4.204-3.602a.563.563 0 01.321-.988l5.518-.442a.563.563 0 00.475-.345L11.48 3.5z"/>
                                            </svg>`);
                }
            }
        })
    });
}

getMomoTransaction = (url) => {
    // $.(url+`/checkout/pay`, function (data) {
    //     $("#momoQr").attr("src", data)
    // })
    $.ajax({
        url: url+`/checkout/pay`,
        method: "post",
        dataType: "json",
        success: data => {
            $("#momoQr").attr("src", data);
        },
    })
}



Object.byString = function(o, s) {
    s = s.replace(/\[(\w+)]/g, '.$1'); // convert indexes to properties
    s = s.replace(/^\./, '');           // strip a leading dot
    var a = s.split('.');
    for (var i = 0, n = a.length; i < n; ++i) {
        var k = a[i];
        if (k in o) {
            o = o[k];
        } else {
            return;
        }
    }
    return o;
}
