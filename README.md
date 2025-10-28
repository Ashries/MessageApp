Miten ohjelmaa käytetään ? Kuinka asiakas saa luotua käyttäjätunnuksen ja kuinka tämän käyttäjätunnuksen avulla sovelluksen muita toimintoja pystyy käyttämään. Käydään sen jälkeen projektin rakkenne ja mennään syvään mitä tiettyssä kansioissa on 

Viestiappi on backend-viestintäsovellus, joka mahdollistaa käyttäjien lähettää julkisia ja yksityisviestejä. Sovellus on toteutettu ASP.NET Core Web API -tekniikalla.
Ohjemaa käynistetään visual studiossa F5 Sovellus pitäisi käynistyy automaattisistei sellaimissa. Voit tarkistaa nuget controllista: dotnet list package mitä paketteja oli asennettu tähän projektiin. 


Testaus postmanissa ja 
Rekisteröi uusi käyttäjä:

Valitse POST /api/users/register

-Syötä käyttäjätiedot

-Valitse POST /api/users/login

-Voit nyt kirjautua sisään käytttäjään 

-POST /api/messages 



Viestien kähettäminen:

JulkinenViesti
POST /api/messages
X-API-Key: your-secret-api-key-12345
X-User-Id: 1
Content-Type: application/json


  "title": "Hei maailma!",
  "content": "Tämä on julkinen viesti",
  "receiverId": null




Private viesti
Posti https://localhost:7044/api/messages
Muokkaa headers
Content-Type: application/json
X-API-Key: your-secret-api-key-12345
X-User-Id: 1

  "title": "Salainen viesti kakkoselle",
  "content": "Tämä viesti on vain sinulle",
  "receiverId": 2,
  "previousMessageId": null


Viestien hakeminen postaminissa testausta varten

GET /api/messages/public          # Kaikki julkiset viestit
GET /api/messages/private         # Käyttäjän yksityisviestit
GET /api/messages/thread/1        # Viestiketju


Käydään läpi tekniiset asiat projektissa

Projektin kansio rakenne
MessageApp/
├── Controllers/          # API-kontrollerit
├── Models/              # Tietomallit
├── DTOs/                # Data Transfer Objects
├── Services/            # Liiketoimintalogiikka
├── Repositories/        # Tietokantakäsittely
├── Interfaces/          # Rajapinnat
├── Data/               # Tietokantakonteksti
├── Middleware/         # HTTP-middleware
└── Program.cs          # Sovelluksen konfiguraatio
Jokainen kansio on erikoistunut tiettyyn tehtävään, mikä tekee koodista helpommin ylläpidettävää.




Vaihettais kuvaus ohjelmasta

Vaihe 1: HTTP-pyyntö saapuu

Käyttäjä lähettää POST-pyynnön osoitteeseen /api/messages
Pyynnössä on:
API-avain headerissa (X-API-Key)
Käyttäjän ID headerissa (X-User-Id)
Viestin data JSON-muodossa


Vaihe 2: Middleware-käsittely
ApiKeyMiddleware: Tarkistaa että API-avain on oikea
RequestLoggingMiddleware: Kirjaa tiedot pyynnöstä (kuka, mitä, milloin)
ExceptionHandlingMiddleware: Ottaa kiinni mahdolliset virheet


Vaihe 3: Kontrolleri
MessagesController vastaanottaa pyynnön
Hakee käyttäjätiedot X-User-Id headerista
Validoida että kaikki tarvittava data on annettu



Vaihe 4: Palvelukerros
MessageService käsittelee liiketoimintalogiikan:
Tarkistaa että lähettäjä on olemassa
Tarkistaa että vastaanottaja on olemassa (jos yksityisviesti)
Varmistaa että edellinen viesti on olemassa (jos vastaus)




Vaihe 5: Tietokantakäsittely
MessageRepository tallentaa viestin:
Luo uuden Message-olion
Asettaa ajankohdan (SentAt)
tallentaa tietokantaan Entity Frameworkin avulla



Vaihe 6: Vastaus
Sovellus palauttaa HTTP 201 Created -statuskoodin
Vastauksessa on luodun viestin tiedot:
Viestin ID
Otsikko ja sisältö
Lähettäjän tiedot
Vastaanottajan tiedot (jos yksityisviesti)
