---
title: AI-APP-ST304 - 02 - Lab Setup - Confirm the app is running
description: Lab instructions for resource provisioning of the MSReady 19 AI-APP-ST304 technical lab session
author: mcerreto
---
# Lab Setup - Confirm the app is running

## 1. Get the Application Insights Instrumentation Key

The lab makes use of Application Insights to get and analyze different metrics of the applications deployed within the AKS cluster.  
After you have created the resource, you get its instrumentation key and use that to configure the SDK in the applications. The resource key links the telemetry to the resource.  
In the previous lab, by executing the step 3, you have already created the Application Insights workspace; now we can obtain the instrumentation key using Azure CLI.

1. Execute the following command:

    ```azurecli-interactive
    az resource show -g "MSReady19-Lab-RG" -n "MSReady19-AI-APP-ST304-AppInsights" --resource-type "Microsoft.Insights/components" --query properties.InstrumentationKey
    ```

    the output will be similar to this:

    ![alt text](imgs/mod_02_img_01.png "Getting of the Application Insights Instrumentation Key")

2. Take note of the GUID reported in the execution output of the previous command

## 2. Add the Instrumentation Key to the K8S secret file

A Secret is an object that contains a small amount of sensitive data such as a password, a token, or a key. Such information might otherwise be put in a Pod specification or in an image; putting it in a Secret object allows for more control over how it is used, and reduces the risk of accidental exposure.

All the .NET Core applications included in this Lab will reference the Instrumentation Key in order to be able to push telemetry data into the Application Insights workspace. The key is obtained by reading an enviroment variable, present within the POD env, that is directly bound to a secret in the k8s service creation phase.

All the K8S deployment files that you will use in the next few steps contains a strict reference to a Kubernetes secret that needs to be already created within the cluster.

So, let's proceed to edit the secret yaml file:

1. Open the k8s secret file in VS Code by executing in the command prompt the following command:

    ```dos
    code C:\Labs\K8sConfigurations\secrets\instrumentationkey.yaml
    ```

2. Add the key, obtained in the previous step, to the _instrumentationkey_ yaml property, as shown in the following images

    ![alt text](imgs/mod_02_img_02.png "Replacing of the instrumentation key value")

    in order to have the following final result

     ![alt text](imgs/mod_02_img_03.png "Instrumentation key value replaced")

3. Save the edited file by using Ctrl+S shortcut or File->Save menu item, then close VS Code

## 3. Create the secret using the k8s yaml file

The secret key will be created within the cluster using the _kubectl_ CLI

1. Execute the following command and wait for the execution

    ```dos
    kubectl create -f C:\Labs\K8sConfigurations\secrets\instrumentationkey.yaml
    ```

    the output will be:

    ```dos
    secret "instrumentationkey" created
    ```
2. Double check the presence of the secret by executing the following command:

    ```dos
    kubectl get secrets
    ```

    that will return an output similar to this:

    ```plain
    NAME                  TYPE                                  DATA   AGE
    appinsightskey        Opaque                                1      92m
    default-token-d9qrw   kubernetes.io/service-account-token   3      164m
    ```

## 4. Deploy the Book Service V1

1. Deploy the Book Service Web API using _/k8sconfigurations/default/bookservice-v1.yaml_ file trought _kubectl_ CLI, by executing:

    ```dos
    kubectl create -f C:\Labs\K8sConfigurations\default\bookservice-V1.yaml
    ```

    that will return an output similar to this:

    ```dos
    deployment.apps/bookservice created
    service/bookservice created
    ```

2. Check if the BookService _pod_ is correctly running by executing:

    ```dos
    kubectl get pods
    ```

    that will return an output similar to this:

    ```plain
    NAME                           READY   STATUS    RESTARTS   AGE
    bookservice-5bdd9968b5-trrxx   1/1     Running   0          11s
    ```

    indicating _running_ as current status.

3. Check if the BookService _service_ is correctly configured and a cluster IP has been successfully assigned by executing:

    ```dos
    kubectl get service
    ```

    that will return the following output:

    ```plain
    NAME          TYPE        CLUSTER-IP    EXTERNAL-IP   PORT(S)   AGE
    bookservice   ClusterIP   10.0.98.140   <none>        80/TCP    23s
    kubernetes    ClusterIP   10.0.0.1      <none>        443/TCP   20h
    ```

## 5. Deploy the BookInfo SPA

1. Deploy the BookInfo Single Page Application using _/k8sconfigurations/default/bookinfo-spa.yaml_ file trought _kubectl_ CLI, by executing:

    ```dos
    kubectl create -f C:\Labs\K8sConfigurations\default\bookinfo-spa.yaml
    ```

    that will return an output similar to this:

    ```plain
    deployment.apps/bookinfo-spa created
    service/bookinfo-spa created
    ```

2. As the deployment file, the pods related to the BookInfo SPA needs to have 2 replicas, so we need to check if both replicas are currently running by executing:

    ```dos
    kubectl create -f C:\Labs\K8sConfigurations\default\bookinfo-spa.yaml
    ```

    that will return an output similar to this:

    ```plain
    NAME                            READY   STATUS    RESTARTS   AGE
    bookinfo-spa-57bdd84f98-8jmm9   2/2     Running   0          44s
    bookservice-5bdd9968b5-trrxx    1/1     Running   0          28m
    ```

    indicating that 2 of 2 replicas are have status as _running_

3. Since, we need to expose the Web Application to external users, BookInfo SPA deployment file also contains the definition for a _LoadBalancer_ k8s service that provides the configuration of an external IP that will be used to reach the sidecar proxy and then the web application. You check the service status and retrieve the externa IP by executing:

    ```dos
    kubectl get service
    ```

    that will return an output similar to this:

    ```plain
    NAME           TYPE           CLUSTER-IP    EXTERNAL-IP      PORT(S)        AGE
    bookinfo-spa   LoadBalancer   10.0.75.9     104.42.174.161   80:30654/TCP   7m4s
    bookservice    ClusterIP      10.0.98.140   <none>           80/TCP         35m
    kubernetes     ClusterIP      10.0.0.1      <none>           443/TCP        20h
    ```

    reporting, in that case, _104.42.174.161_ as the public IP you should use.

## 6. Check the BookInfo SPA using the browser

1. Open Edge Browser;

2. Open the URL _http://\[**EXTERNAL_IP_PLACEHOLDER**\] replacing the placeholder with the public ip obtained in the previous step, then the home page of the single page application will be shown

    ![alt text](imgs/mod_02_img_04.png "BookInfo SPA Home Page")

3. Click on the **Books** menu item at left to open the Books portal functionality

    ![alt text](imgs/mod_02_img_05.png "Book Portal")

    If you are able to successfully see the book list and the related reviews, you can confirm that also the BookService API is working correctly.

4. Make some clicks on bookitem in order to generate some HTTP traffic between the SPA and the Web API.

## 7. Check if telemetry is present trough AppInsights

1. Open Edge Browser;
2. Open Azure Portal at http://portal.azure.com;
3. Sign-In using Cloud Slice credential provided within this Lab;
4. Browse all Resource Groups by click on _Resource groups_ menu item on the left bar;
5. You should see the two resource groups created in the Lab 1, as the following screenshot shows:

    ![alt text](imgs/mod_02_img_06.png "Resource Groups")

6. Click on the RG named _MSReady19-Lab-RG_;
7. Access to the Application Insights resource by clicking on _MSReady19-AI-APP-ST304-AppInsights_

    ![alt text](imgs/mod_02_img_07.png "Application Insights")

8. Click on _Analytics_ button to access the custom query editor

    ![alt text](imgs/mod_02_img_08.png "Application Insights Analytics")

9. Type the following query

    ```loganalytics
    requests
     | where resultCode == 200
    ```

    into the query editor as shown in the following screenshot

    ![alt text](imgs/mod_02_img_09.png "Application Insights Query")

    in order to retrieve info on requests with 200 result code handled in the last 24h;

10. Click on _Run_ button, then you should see the results

    ![alt text](imgs/mod_02_img_10.png "Application Insights Query Results")

    indicating that the telemetry is correctly working.