using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.AI.Agents.Persistent;
using Azure.Core;
using Azure.Identity;

namespace AiFoundryExp;

public class AgentFactory
{
    private readonly PersistentAgentsClient _client;
    private readonly string _modelDeployment;

    public AgentFactory(string endpoint, string modelDeploymentName, TokenCredential? credential = null)
    {
        credential ??= new DefaultAzureCredential();
        _client = new PersistentAgentsClient(endpoint, credential);
        _modelDeployment = modelDeploymentName;
    }

    public async Task<PersistentAgent> CreateAgentAsync(
        string name,
        string instructions,
        IEnumerable<ToolDefinition>? tools = null,
        ToolResources? resources = null,
        float? temperature = null,
        float? topP = null)
    {
        tools ??= new List<ToolDefinition>();

        await foreach (PersistentAgent existing in _client.Administration.GetAgentsAsync())
        {
            if (string.Equals(existing.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return existing;
            }
        }

        return await _client.Administration.CreateAgentAsync(
            model: _modelDeployment,
            name: name,
            instructions: instructions,
            tools: tools,
            toolResources: resources,
            temperature: temperature,
            topP: topP);
    }
}
