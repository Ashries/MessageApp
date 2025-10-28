Miten ohjelmaa käytetään ? Kuinka asiakas saa luotua käyttäjätunnuksen ja kuinka tämän käyttäjätunnuksen avulla sovelluksen muita toimintoja pystyy käyttämään. Käydään sen jälkeen projektin rakkenne ja mennään syvään mitä tiettyssä kansioissa on 

Viestiappi on backend-viestintäsovellus, joka mahdollistaa käyttäjien lähettää julkisia ja yksityisviestejä. Sovellus on toteutettu ASP.NET Core Web API -tekniikalla.
Ohjemaa käynistetään visual studiossa F5 Sovellus pitäisi käynistyy automaattisistei sellaimissa. Voit tarkistaa nuget controllista: dotnet list package mitä paketteja oli asennettu tähän projektiin. 

Voit

Testaus postmanissa 
Rekisteröi uusi käyttäjä:

Valitse POST /api/users/register

Syötä käyttäjätiedot

Valitse POST /api/users/login

Voit nyt kirjautua sisään käytttäjään 

POST /api/messages 

Viestien kähettäminen:

POST /api/messages
X-API-Key: your-secret-api-key-12345
X-User-Id: 1
Content-Type: application/json

{
  "title": "Hei maailma!",
  "content": "Tämä on julkinen viesti",
  "receiverId": null
}

Viestien hakeminen postaminissa testausta varten

GET /api/messages/public          # Kaikki julkiset viestit
GET /api/messages/private         # Käyttäjän yksityisviestit
GET /api/messages/thread/1        # Viestiketju


