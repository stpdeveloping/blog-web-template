<%@ Page Language="C#" enableSessionState="true" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Blog.index" %>
<% 
    %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>PCinfo - Strona Główna</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bulma/0.6.2/css/bulma.min.css">
    <script src="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.3.1.min.js"></script>
    <script>
        $(document).ready(function () {
            $("li").click(function () {
                $("li").removeClass("is-active");
                $(this).addClass("is-active");
            });
            $.ajax({
            type: "POST",
            url: "index.aspx/SprawdzenieSesji",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccess
            });
            function OnSuccess(response) {
                for (i = 0; i < 3; i++)
                        $("#pan" + i).empty();
                if (response.d[0] != null) {
                    var zalogowany = response.d[0];
                    $('#pan0').append("<p class='is-small'>Zalogowałeś się jako <strong>" + zalogowany + "</strong></p>");
                    $('#pan1').append("<div class='button is-info is-small' onclick='wylogowanie()'>Wyloguj</div>");
                }
                else {
                    $('#pan0').append("<input class='input is-small is-rounded' id='log' type='text' placeholder='Nazwa użytkownika...' />");
                    $('#pan1').append("<input class='input is-small is-rounded' id='pass' type='password' placeholder='Hasło...' />");
                    $('#pan2').append("<div class='button is-info is-small' onclick='logowanie()'>Zaloguj</div>");
                }
                switch (response.d[2]) {
                    case 'o-mnie':
                        oMnie();
                        break;
                    case 'newsy':
                        //Newsy();
                        break;
                    case 'cpu-rank':
                        //RankingCPU();
                        break;
                    case 'gpu-rank':
                        //RankingGPU();
                        break;
                    case 'reg':
                        Rejestracja();
                        break;
                }
            }
        });
        function ajaxNumerStrony(i) {
            $.ajax({
            type: "POST",
            url: "index.aspx/SprawdzenieAktywnejStrony",
            data: '{"numer_strony":"' + i + '"}', 
            contentType: "application/json; charset=utf-8",
            dataType: "json"
            });
        }
        function oMnie() {
            ajaxNumerStrony(1);
            $("#contentStrony").text("");
            $('#contentStrony')
                .append("<section class ='section'><h3 class='has-text-centered'>Bartłomiej Stasiak</h3>" +
                "<p class='has-text-centered'><img src='http://oi65.tinypic.com/zmjos2.jpg' /></p>" +
                "<p>Witam na mojej stronie. Jestem studentem VI semestru na WSTI i pochodzę z Katowic. " +
                "Zamieszczam tu nowinki ze świata podzespołów komputerowych.</p></section>");
        }
        function Rejestracja() {
            ajaxNumerStrony(5);
            $('#contentStrony').empty();
            $('#contentStrony').append($("<div class='kontener'>"));
            var dane = ["Nazwa użytkownika: ", "Hasło: ", "Powtórz hasło: ", "Miasto: ", "E-mail: "];
            for (i = 0; i < dane.length; i++)
            {
                var typ = "'text'";
                if (dane[i].includes("hasło") || dane[i].includes("Hasło"))
                    typ = "'password'";
                $('.kontener').append($("<div class = 'columns'>")
                    .append($("<div class='column is-one-quarter'>")
                        .append("<label class='label has-text-right'>" + dane[i] + "</label>"))
                    .append($("<div class='column is-one-quarter'>")
                        .append("<input class='input' id='" + i + "' type=" + typ + "/>"))
                    .append($("<div class='column is-one-quarter'>"))
                    .append($("<div class='column is-one-quarter'>"))
                );
            }
            $('.kontener').append($("<div class = 'columns'>")
                .append($("<div class='column is-one-quarter'>"))
                .append($("<div class='column is-one-quarter'>")
                    .append("<button class='button is-info is-fullwidth' id='przyciskrejestracji' onclick='sprawdzRejestracje()'>Zarejestruj</button>"))
                .append($("<div class='column is-one-quarter'>"))
                .append($("<div class='column is-one-quarter'>"))
            );
            $('.kontener').wrap($("<section class='section'>"));
        }
        function sprawdzRejestracje() {
            var znacznik = 0;
            var input_nazwa = $('#0').val();
            var input_haslo = $('#1').val();
            var input_miasto = $('#3').val();
            var input_email = $('#4').val();
            var walidacja_hasla = $('#2').val();
            for (i = 0; i < 3; i++) {
                var x = $("#" + i).val();
                if (x.length == 0) {
                    $("#" + i).addClass("is-danger");
                    znacznik = 1;
                }
                else $("#" + i).removeClass("is-danger");
                if (i == 2 && znacznik == 1)
                        return;
            }

            if (input_haslo != walidacja_hasla) {
                $('#1').addClass("is-danger");
                $('#2').addClass("is-danger");
                return;
            }
        $.ajax({
            type: "POST",
            url: "index.aspx/RejestracjaUzytkownika",
            data: '{"nick":"' + input_nazwa + '","password":"' + input_haslo + '","city":"' + input_miasto + '","email":"' + input_email + '"}', 
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccess
            });
            function OnSuccess(response) {
                var odpowiedz = response.d;
                if (odpowiedz === "zly-nick") {
                    $('#0').addClass("is-danger");
                    alert("Wprowadzona nazwa użytkownika jest już zarezerwowana!. Wybierz inną.");
                }
                if (response.d === "zly-email") {
                    $('#4').addClass("is-danger");
                    alert("Wprowadzony adres mailowy jest już zarezerwowany!");
                }
                if (response.d === "prawidlowe-dane") {
                    for (i = 0; i < 5; i++)
                        $("#" + i).addClass("is-success");
                    $('#przyciskrejestracji').prop('disabled', true);
                }

        }
        }
        function logowanie() {
            var input_nazwa = $('#log').val();
            var input_haslo = $('#pass').val();
            $.ajax({
                type: "POST",
                url: "index.aspx/LogowanieUzytkownika",
                data: "{nick:'" + input_nazwa + "', password:'" + input_haslo + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccess
            });
            function OnSuccess(response) {
                if (response.d == true) location.reload();
                else alert("Nieprawidłowy login lub hasło.");
            }
        }
        function wylogowanie() {
            $.ajax({
                type: "POST",
                url: "index.aspx/UsuwanieSesji",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccess
            });
            function OnSuccess() {
                location.reload();
            }
        }
    </script>
</head>
<body>
    <section class="hero is-primary">
        <div class="hero-body">
    <div class="container">
      <h1 class="title has-text-centered">
        PCinfo
      </h1>
    </div>
    </div>
    </section>


    <section class="section">
        <div class="columns is-multiline is-mobile">
              <div class="column is-one-fifth" id ="pan0"><input class="input is-small is-rounded" id="log" type="text" placeholder="Nazwa użytkownika..." /></div>
              <div class="column is-one-fifth" id="pan1"><input class="input is-small is-rounded" id="pass" type="password" placeholder="Hasło..." /></div>
              <div class="column is-one-fifth" id ="pan2"><div class="button is-info is-small" onclick="logowanie()">Zaloguj</div></div>
              <div class="column is-two-fifths"></div>
        </div>

        <div class="tabs">
  <ul>
    <li onclick="oMnie()"><a>O mnie</a></li>
    <li onclick="Newsy()"><a>Newsy</a></li>
    <li onclick="rankingCPU()"><a>Ranking CPU</a></li>
    <li onclick="rankingGPU()"><a>Ranking GPU</a></li>
    <li onclick="Rejestracja()"><a>Rejestracja</a></li>
  </ul>
</div>
            <section class="content" id="contentStrony">
                <section class="section">
                    <div class="kontener">
                        </div>
                </section>
</section>
         </section>

    <footer class="footer">
        <div class="container">
            <div class="content">
                <div class="columns">
                    <div class="column has-text-centered">
                        <a href="https://bulma.io">
                        <img src="https://bulma.io/images/made-with-bulma.png" alt="Made with Bulma" width="128" height="24">
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </footer>


</body>
</html>
