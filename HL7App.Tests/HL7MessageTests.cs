using HL7App.Models;
using System;
using System.IO;
using NUnit.Framework;

namespace HL7App.Tests;

public class HL7MessageTests
{
    private HL7Message a01Message;
    private HL7Message a04Message;
    [SetUp]
    public void Setup()
    {
        string a01 = File.ReadAllText("HL7Messages/A01Message.txt");        
        string a04 = File.ReadAllText("HL7Messages/A04Message.txt");
        a01Message = new HL7Message(a01);
        a04Message = new HL7Message(a04);
    }

    [Test]
    public void TestMessageId()
    {
        Assert.That(a01Message.MessageId, Is.EqualTo("599102"));        
    }

    [Test]
    public void TestMessageId_NotEqual()
    {
        Assert.That(a04Message.MessageId, Is.Not.EqualTo("599102"));
    }

    [Test]
    public void TestMessageTypeComponent()
    {
        Assert.That(a01Message.MessageType, Is.EqualTo("ADT"));
        Assert.That(a01Message.MessageEvent, Is.EqualTo("A01"));
    }
    [Test]
    public void TestMessageTypeComponent_A04()
    {
        Assert.That(a04Message.MessageType, Is.EqualTo("ADT"));
        Assert.That(a04Message.MessageEvent, Is.Not.EqualTo("A01"));
    }
    [Test]
    public void TestGetPatientName()
    {
        Assert.That(a01Message.GetValueFromMessage("PID", 5), Is.EqualTo("DUCK^DONALD^D"));
    }
    [Test]
    public void TestGetPatientName_NotEqual()
    {
        Assert.That(a04Message.GetValueFromMessage("PID", 5), Is.Not.EqualTo("DUCK^DONALD^D"));
    }
}
