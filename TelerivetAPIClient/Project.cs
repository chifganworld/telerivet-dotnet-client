
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Telerivet.Client
{
/**
    Represents a Telerivet project.
    
    Provides methods for sending and scheduling messages, as well as
    accessing, creating and updating a variety of entities, including contacts, messages,
    scheduled messages, groups, labels, phones, services, and data tables.
    
    Fields:
    
      - id (string, max 34 characters)
          * ID of the project
          * Read-only
      
      - name
          * Name of the project
          * Updatable via API
      
      - timezone_id
          * Default TZ database timezone ID; see
              <http://en.wikipedia.org/wiki/List_of_tz_database_time_zones>
          * Read-only
      
      - url_slug
          * Unique string used as a component of the project's URL in the Telerivet web app
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this project
          * Updatable via API
      
      - organization_id (string, max 34 characters)
          * ID of the organization this project belongs to
          * Read-only
*/

public class Project : Entity
{
    /**
        Sends one message (SMS, voice call, or USSD request).
    */
    public async Task<Message> SendMessageAsync(JObject options)
    {
        return new Message(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/messages/send", options));
    }

    /**
        Sends a text message (optionally with mail-merge templates) or voice call to a group or a
        list of up to 500 phone numbers
    */
    public async Task<Broadcast> SendBroadcastAsync(JObject options)
    {
        return new Broadcast(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/send_broadcast", options));
    }

    /**
        Sends up to 100 different messages in a single API request. This method is significantly
        faster than sending a separate API request for each message.
    */
    public async Task<JObject> SendMultiAsync(JObject options)
    {
        return (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/send_multi", options);
    }

    /**
        (Deprecated) Send a message a to group or a list of phone numbers.
        This method is only needed to maintain backward compatibility with
        code developed using previous versions of the client library.
        Use `sendBroadcast` or `sendMulti` instead.
    */
    public async Task<JObject> SendMessagesAsync(JObject options)
    {
        return (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/messages/send_batch", options);
    }

    /**
        Schedules a message to a group or single contact. Note that Telerivet only sends scheduled
        messages approximately once every 15 seconds, so it is not possible to control the exact
        second at which a scheduled message is sent.
    */
    public async Task<ScheduledMessage> ScheduleMessageAsync(JObject options)
    {
        return new ScheduledMessage(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/scheduled", options));
    }

    /**
        Add an incoming message to Telerivet. Acts the same as if the message was received by a
        phone. Also triggers any automated services that apply to the message.
    */
    public async Task<Message> ReceiveMessageAsync(JObject options)
    {
        return new Message(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/messages/receive", options));
    }

    /**
        Retrieves OR creates and possibly updates a contact by name or phone number.
        
        If a phone number is provided, by default, Telerivet will search for
        an existing contact with that phone number (including suffix matches to allow finding
        contacts with phone numbers in a different format). If a phone number is not provided but a
        name is provided, Telerivet will search for a contact with that exact name (case
        insensitive). This behavior can be modified by setting the `lookup_key` parameter to look up
        a contact by another field, including a custom variable.
        
        If no existing contact is found, a new contact will be created.
        
        Then that contact will be updated with any parameters provided
        (`name`, `phone_number`, `vars`, `default_route_id`, `send_blocked`, `add_group_ids`,
        `remove_group_ids`).
    */
    public async Task<Contact> GetOrCreateContactAsync(JObject options = null)
    {
        return new Contact(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/contacts", options));
    }

    /**
        Creates and/or updates up to 200 contacts in a single API call. When creating or updating a
        large number of contacts, this method is significantly faster than sending a separate API
        request for each contact.
        
        By default, if the phone number for any contact matches an existing
        contact, the existing contact will be updated with any information provided. This behavior
        can be modified by setting the `lookup_key` parameter to look up contacts by another field,
        including a custom variable.
        
        If any contact was not found matching the provided `lookup_key`, a
        new contact will be created.
    */
    public async Task<JObject> ImportContactsAsync(JObject options)
    {
        return (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/import_contacts", options);
    }

    /**
        Queries contacts within the given project.
    */
    public APICursor<Contact> QueryContacts(JObject options = null)
    {
        return api.NewCursor<Contact>(GetBaseApiPath() + "/contacts", options);
    }

    /**
        Retrieves the contact with the given ID.
    */
    public async Task<Contact> GetContactByIdAsync(string id)
    {
        return new Contact(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/contacts/" + id));
    }

    /**
        Initializes the Telerivet contact with the given ID without making an API request.
    */
    public Contact InitContactById(string id)
    {
        return new Contact(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries phones within the given project.
    */
    public APICursor<Phone> QueryPhones(JObject options = null)
    {
        return api.NewCursor<Phone>(GetBaseApiPath() + "/phones", options);
    }

    /**
        Retrieves the phone with the given ID.
    */
    public async Task<Phone> GetPhoneByIdAsync(string id)
    {
        return new Phone(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/phones/" + id));
    }

    /**
        Initializes the phone with the given ID without making an API request.
    */
    public Phone InitPhoneById(string id)
    {
        return new Phone(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries messages within the given project.
    */
    public APICursor<Message> QueryMessages(JObject options = null)
    {
        return api.NewCursor<Message>(GetBaseApiPath() + "/messages", options);
    }

    /**
        Retrieves the message with the given ID.
    */
    public async Task<Message> GetMessageByIdAsync(string id)
    {
        return new Message(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/messages/" + id));
    }

    /**
        Initializes the Telerivet message with the given ID without making an API request.
    */
    public Message InitMessageById(string id)
    {
        return new Message(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries broadcasts within the given project.
    */
    public APICursor<Broadcast> QueryBroadcasts(JObject options = null)
    {
        return api.NewCursor<Broadcast>(GetBaseApiPath() + "/broadcasts", options);
    }

    /**
        Retrieves the broadcast with the given ID.
    */
    public async Task<Broadcast> GetBroadcastByIdAsync(string id)
    {
        return new Broadcast(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/broadcasts/" + id));
    }

    /**
        Initializes the Telerivet broadcast with the given ID without making an API request.
    */
    public Broadcast InitBroadcastById(string id)
    {
        return new Broadcast(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries groups within the given project.
    */
    public APICursor<Group> QueryGroups(JObject options = null)
    {
        return api.NewCursor<Group>(GetBaseApiPath() + "/groups", options);
    }

    /**
        Retrieves or creates a group by name.
    */
    public async Task<Group> GetOrCreateGroupAsync(string name)
    {
        return new Group(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/groups", Util.Options("name", name)));
    }

    /**
        Retrieves the group with the given ID.
    */
    public async Task<Group> GetGroupByIdAsync(string id)
    {
        return new Group(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/groups/" + id));
    }

    /**
        Initializes the group with the given ID without making an API request.
    */
    public Group InitGroupById(string id)
    {
        return new Group(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries labels within the given project.
    */
    public APICursor<Label> QueryLabels(JObject options = null)
    {
        return api.NewCursor<Label>(GetBaseApiPath() + "/labels", options);
    }

    /**
        Gets or creates a label by name.
    */
    public async Task<Label> GetOrCreateLabelAsync(string name)
    {
        return new Label(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/labels", Util.Options("name", name)));
    }

    /**
        Retrieves the label with the given ID.
    */
    public async Task<Label> GetLabelByIdAsync(string id)
    {
        return new Label(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/labels/" + id));
    }

    /**
        Initializes the label with the given ID without making an API request.
    */
    public Label InitLabelById(string id)
    {
        return new Label(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries data tables within the given project.
    */
    public APICursor<DataTable> QueryDataTables(JObject options = null)
    {
        return api.NewCursor<DataTable>(GetBaseApiPath() + "/tables", options);
    }

    /**
        Gets or creates a data table by name.
    */
    public async Task<DataTable> GetOrCreateDataTableAsync(string name)
    {
        return new DataTable(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/tables", Util.Options("name", name)));
    }

    /**
        Retrieves the data table with the given ID.
    */
    public async Task<DataTable> GetDataTableByIdAsync(string id)
    {
        return new DataTable(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/tables/" + id));
    }

    /**
        Initializes the data table with the given ID without making an API request.
    */
    public DataTable InitDataTableById(string id)
    {
        return new DataTable(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries scheduled messages within the given project.
    */
    public APICursor<ScheduledMessage> QueryScheduledMessages(JObject options = null)
    {
        return api.NewCursor<ScheduledMessage>(GetBaseApiPath() + "/scheduled", options);
    }

    /**
        Retrieves the scheduled message with the given ID.
    */
    public async Task<ScheduledMessage> GetScheduledMessageByIdAsync(string id)
    {
        return new ScheduledMessage(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/scheduled/" + id));
    }

    /**
        Initializes the scheduled message with the given ID without making an API request.
    */
    public ScheduledMessage InitScheduledMessageById(string id)
    {
        return new ScheduledMessage(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries services within the given project.
    */
    public APICursor<Service> QueryServices(JObject options = null)
    {
        return api.NewCursor<Service>(GetBaseApiPath() + "/services", options);
    }

    /**
        Retrieves the service with the given ID.
    */
    public async Task<Service> GetServiceByIdAsync(string id)
    {
        return new Service(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/services/" + id));
    }

    /**
        Initializes the service with the given ID without making an API request.
    */
    public Service InitServiceById(string id)
    {
        return new Service(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries custom routes that can be used to send messages (not including Phones).
    */
    public APICursor<Route> QueryRoutes(JObject options = null)
    {
        return api.NewCursor<Route>(GetBaseApiPath() + "/routes", options);
    }

    /**
        Gets a custom route by ID
    */
    public async Task<Route> GetRouteByIdAsync(string id)
    {
        return new Route(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/routes/" + id));
    }

    /**
        Initializes a custom route by ID without making an API request.
    */
    public Route InitRouteById(string id)
    {
        return new Route(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Returns an array of user accounts that have access to this project. Each item in the array
        is an object containing `id`, `email`, and `name` properties. (The id corresponds to the
        `user_id` property of the Message object.)
    */
    public async Task<JArray> GetUsersAsync()
    {
        return (JArray) await api.DoRequestAsync("GET", GetBaseApiPath() + "/users");
    }

    /**
        Saves any fields or custom variables that have changed for the project.
    */
    public override async Task SaveAsync()
    {
        await base.SaveAsync();
    }

    public string Id
    {
      get {
          return (string) Get("id");
      }
    }

    public String Name
    {
      get {
          return (String) Get("name");
      }
      set {
          Set("name", value);
      }
    }

    public String TimezoneId
    {
      get {
          return (String) Get("timezone_id");
      }
    }

    public String UrlSlug
    {
      get {
          return (String) Get("url_slug");
      }
    }

    public string OrganizationId
    {
      get {
          return (string) Get("organization_id");
      }
    }

    public override string GetBaseApiPath()
    {
        return "/projects/" + Id + "";
    }

    public Project(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }
}

}
