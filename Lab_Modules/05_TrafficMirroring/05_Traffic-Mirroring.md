# Lab - Traffic Mirroring

## 1. Create the mirrored service and deployment

Open a command prompt or a powershell from the folder "_K8sConfigurations/mirroring_"

1. Execute the following commands:

    `kubectl apply -f bookservice-V2-mirroring.yaml` 
    and then
    `kubectl get services; kubectl get deployments` 
    
    and you should get the following output:
   ![image.png](/imgs/image-a8e9d79a-18bd-44ac-9cb0-f0ac028221a6.png)

2. At this point both mirrored deployment (bookservicemirror) and user facing deployment (bookservice) are configured with the same image (readymirroring/bookservice). Now you can browse the web application or invoke the _poller.ps1_ script used in the previous labs.
    ![image.png](/imgs/image-acc4a3b4-a429-4243-b5e0-3cb1c07850f8.png)

3. Wait a couple of minutes, needed for Azure Application Insights to collect telemetry, and paste the content of the "_LogAnalyticsQuery.md_" file into Azure Log Analytics. 
  `requests
| where customDimensions["VersionTag"] contains "MIR-"
| summarize duration = avg(duration), requestCount = count() by name, podVersion = tostring(customDimensions["VersionTag"]), resultCode 
| sort by name, podVersion` 

   Then hit "Run" query and you should get something similar to the following image:
    ![image.png](/imgs/image-d02fbdc2-5510-4204-9809-706954155ed9.png)

4. We are done with our first traffic mirroring! You can see from the query results above that, as we expected, result codes and duration are very close between mirrored and user facing service. 
In order to achieve that, we tagged traffic coming from user facing service with the attribute "podVersion" = "**V1MIR-BookService**" and the traffic coming from mirrored service with the attribute "podVersion" = "**V2MIR-BookService**"
_Please expect few differences in number between your query results and the above image_.

 5. How does it work?
The front end reverse proxy, [Envoy](https://www.envoyproxy.io/), has a very useful configuration that allows to send traffic to a live cluster and a mirror cluster: the traffic is sent to the mirror cluster in a fire and forget way, which means that Envoy doesn't wait for an answer from the mirror cluster. 
You can find the mirror configuration in the file "_Sidecars\default\default-sidecar.yaml_".  Below an excerpt of file:
    ![image.png](/imgs/image-0f5fe834-0adc-4018-8aeb-ab2296b303f1.png =400x300) 
At line 25 and 26, we configure envoy so that every request to "_bookservice_" cluster must be mirrored to "_bookservicemirror_" cluster.  
Then the configuration of two clusters is straightforward (note how the addresses correspond to the kubernetes services names):
![image.png](/imgs/image-1c22b56b-c325-4fe4-a34b-5db9f2e54e74.png  =400x400)

## 2. Introduce a performance decrease in the mirrored service

1. From the same folder, we now rollout a new version of our mirrored bookservice, which introduces a delay while loading book reviews. Type following command:

     `kubectl apply -f bookservice-V3-delays.yaml` 

2. At this point, let's browse again between book reviews from the web page or run the _poller.ps1_ as below:
    ![image.png](/imgs/image-acc4a3b4-a429-4243-b5e0-3cb1c07850f8.png) 

3. Wait a couple of minutes, needed for Azure Application Insights to collect telemetry, and paste the content of the "_LogAnalyticsQuery.md_" file into Azure Log Analytics. 
  `requests
| where customDimensions["VersionTag"] contains "MIR-"
| summarize duration = avg(duration), requestCount = count() by name, podVersion = tostring(customDimensions["VersionTag"]), resultCode 
| sort by name, podVersion` 

   Then hit "Run" query and you should get something similar to the following image:
   ![image.png](/imgs/image-b8616c82-b892-44a2-86ba-9df3e048b002.png)

4. We have anticipated our first problem without impacting real users! You can see from Azure Log Analytics query results that the service with "**V3MIR-BookService**" tag has an average duration of 1,208 milliseconds, while the "**V1MIR-BookService**" (the version real users are seeing) still has an average requests duration of just 15 milliseconds meaning they are not impacted.



## 3. Introduce a fault in the mirrored service

1. From the same folder, we now rollout a new version of our mirrored service, which introduces a fault while loading book reviews with and even Id. Type following command:

     `kubectl apply -f bookservice-V4-fault.yaml` 

2. At this point, let's browse again between book reviews from the web page or run the _poller.ps1_ as below:
    ![image.png](/imgs/image-acc4a3b4-a429-4243-b5e0-3cb1c07850f8.png) 

3. Wait a couple of minutes, needed for Azure Application Insights to collect telemetry, and paste the content of the "_LogAnalyticsQuery.md_" file into Azure Log Analytics. 
  `requests
| where customDimensions["VersionTag"] contains "MIR-"
| summarize duration = avg(duration), requestCount = count() by name, podVersion = tostring(customDimensions["VersionTag"]), resultCode 
| sort by name, podVersion` 

   Then hit "Run" query and you should get something similar to the following image:

    ![image.png](/imgs/image-abf28799-a7e3-4031-b31d-fe60ef532bb8.png)

4. We have prevented real users from experiencing a failure! You can see from log analytics query results that the service with "**V4MIR-BookService**" has several requests with a **500** status code, indicating a failure. Meanwhile, the user facing version of the service, "**V1MIR-BookService**", runs smoothly without incurring in any failure.
    



