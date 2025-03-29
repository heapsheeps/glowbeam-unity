using NUnit.Framework;
using System.Net.Http;
using UnityEngine;

public class WebServerTests
{
    private HttpClient _client;
    private string _baseUrl;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        // In an actual test scenario, you might spin up the server here or
        // rely on the existing instance if it's running in play mode.
        _client = new HttpClient();
        _baseUrl = "http://localhost:8080/api/v1";
    }

    [Test]
    public void TestDisplayInfoEndpoint()
    {
        var response = _client.GetAsync($"{_baseUrl}/display/info").Result;
        Assert.IsTrue(response.IsSuccessStatusCode, "display/info should return success");
        var content = response.Content.ReadAsStringAsync().Result;
        Debug.Log($"display/info response: {content}");
        Assert.IsNotNull(content);
        // Further parse JSON and validate structure if needed
    }

    [Test]
    public void TestShowTestCard()
    {
        var response = _client.GetAsync($"{_baseUrl}/display/testcard/show").Result;
        Assert.IsTrue(response.IsSuccessStatusCode, "testcard/show should return success");
        // Validate that the manager actually set the testcard
        // Possibly check a property in DisplayManager
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
        // If you started the server in Setup, shut it down here
    }
}
