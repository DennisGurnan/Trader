// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: marketdata.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Tinkoff.InvestApi.V1 {
  /// <summary>
  ///Сервис получения биржевой информации:&lt;/br> **1**. свечи;&lt;/br> **2**. стаканы;&lt;/br> **3**. торговые статусы;&lt;/br> **4**. лента сделок.
  /// </summary>
  public static partial class MarketDataService
  {
    static readonly string __ServiceName = "tinkoff.public.invest.api.contract.v1.MarketDataService";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Tinkoff.InvestApi.V1.GetCandlesRequest> __Marshaller_tinkoff_public_invest_api_contract_v1_GetCandlesRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Tinkoff.InvestApi.V1.GetCandlesRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Tinkoff.InvestApi.V1.GetCandlesResponse> __Marshaller_tinkoff_public_invest_api_contract_v1_GetCandlesResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Tinkoff.InvestApi.V1.GetCandlesResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Tinkoff.InvestApi.V1.GetLastPricesRequest> __Marshaller_tinkoff_public_invest_api_contract_v1_GetLastPricesRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Tinkoff.InvestApi.V1.GetLastPricesRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Tinkoff.InvestApi.V1.GetLastPricesResponse> __Marshaller_tinkoff_public_invest_api_contract_v1_GetLastPricesResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Tinkoff.InvestApi.V1.GetLastPricesResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Tinkoff.InvestApi.V1.GetOrderBookRequest> __Marshaller_tinkoff_public_invest_api_contract_v1_GetOrderBookRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Tinkoff.InvestApi.V1.GetOrderBookRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Tinkoff.InvestApi.V1.GetOrderBookResponse> __Marshaller_tinkoff_public_invest_api_contract_v1_GetOrderBookResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Tinkoff.InvestApi.V1.GetOrderBookResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Tinkoff.InvestApi.V1.GetTradingStatusRequest> __Marshaller_tinkoff_public_invest_api_contract_v1_GetTradingStatusRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Tinkoff.InvestApi.V1.GetTradingStatusRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Tinkoff.InvestApi.V1.GetTradingStatusResponse> __Marshaller_tinkoff_public_invest_api_contract_v1_GetTradingStatusResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Tinkoff.InvestApi.V1.GetTradingStatusResponse.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Tinkoff.InvestApi.V1.GetCandlesRequest, global::Tinkoff.InvestApi.V1.GetCandlesResponse> __Method_GetCandles = new grpc::Method<global::Tinkoff.InvestApi.V1.GetCandlesRequest, global::Tinkoff.InvestApi.V1.GetCandlesResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "GetCandles",
        __Marshaller_tinkoff_public_invest_api_contract_v1_GetCandlesRequest,
        __Marshaller_tinkoff_public_invest_api_contract_v1_GetCandlesResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Tinkoff.InvestApi.V1.GetLastPricesRequest, global::Tinkoff.InvestApi.V1.GetLastPricesResponse> __Method_GetLastPrices = new grpc::Method<global::Tinkoff.InvestApi.V1.GetLastPricesRequest, global::Tinkoff.InvestApi.V1.GetLastPricesResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "GetLastPrices",
        __Marshaller_tinkoff_public_invest_api_contract_v1_GetLastPricesRequest,
        __Marshaller_tinkoff_public_invest_api_contract_v1_GetLastPricesResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Tinkoff.InvestApi.V1.GetOrderBookRequest, global::Tinkoff.InvestApi.V1.GetOrderBookResponse> __Method_GetOrderBook = new grpc::Method<global::Tinkoff.InvestApi.V1.GetOrderBookRequest, global::Tinkoff.InvestApi.V1.GetOrderBookResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "GetOrderBook",
        __Marshaller_tinkoff_public_invest_api_contract_v1_GetOrderBookRequest,
        __Marshaller_tinkoff_public_invest_api_contract_v1_GetOrderBookResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Tinkoff.InvestApi.V1.GetTradingStatusRequest, global::Tinkoff.InvestApi.V1.GetTradingStatusResponse> __Method_GetTradingStatus = new grpc::Method<global::Tinkoff.InvestApi.V1.GetTradingStatusRequest, global::Tinkoff.InvestApi.V1.GetTradingStatusResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "GetTradingStatus",
        __Marshaller_tinkoff_public_invest_api_contract_v1_GetTradingStatusRequest,
        __Marshaller_tinkoff_public_invest_api_contract_v1_GetTradingStatusResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Tinkoff.InvestApi.V1.MarketdataReflection.Descriptor.Services[0]; }
    }

    /// <summary>Client for MarketDataService</summary>
    public partial class MarketDataServiceClient : grpc::ClientBase<MarketDataServiceClient>
    {
      /// <summary>Creates a new client for MarketDataService</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public MarketDataServiceClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for MarketDataService that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public MarketDataServiceClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected MarketDataServiceClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected MarketDataServiceClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>
      ///Метод запроса исторических свечей по инструменту.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Tinkoff.InvestApi.V1.GetCandlesResponse GetCandles(global::Tinkoff.InvestApi.V1.GetCandlesRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetCandles(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///Метод запроса исторических свечей по инструменту.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Tinkoff.InvestApi.V1.GetCandlesResponse GetCandles(global::Tinkoff.InvestApi.V1.GetCandlesRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_GetCandles, null, options, request);
      }
      /// <summary>
      ///Метод запроса исторических свечей по инструменту.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Tinkoff.InvestApi.V1.GetCandlesResponse> GetCandlesAsync(global::Tinkoff.InvestApi.V1.GetCandlesRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetCandlesAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///Метод запроса исторических свечей по инструменту.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Tinkoff.InvestApi.V1.GetCandlesResponse> GetCandlesAsync(global::Tinkoff.InvestApi.V1.GetCandlesRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_GetCandles, null, options, request);
      }
      /// <summary>
      ///Метод запроса последних цен по инструментам.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Tinkoff.InvestApi.V1.GetLastPricesResponse GetLastPrices(global::Tinkoff.InvestApi.V1.GetLastPricesRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetLastPrices(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///Метод запроса последних цен по инструментам.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Tinkoff.InvestApi.V1.GetLastPricesResponse GetLastPrices(global::Tinkoff.InvestApi.V1.GetLastPricesRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_GetLastPrices, null, options, request);
      }
      /// <summary>
      ///Метод запроса последних цен по инструментам.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Tinkoff.InvestApi.V1.GetLastPricesResponse> GetLastPricesAsync(global::Tinkoff.InvestApi.V1.GetLastPricesRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetLastPricesAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///Метод запроса последних цен по инструментам.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Tinkoff.InvestApi.V1.GetLastPricesResponse> GetLastPricesAsync(global::Tinkoff.InvestApi.V1.GetLastPricesRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_GetLastPrices, null, options, request);
      }
      /// <summary>
      ///Метод получения стакана по инструменту.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Tinkoff.InvestApi.V1.GetOrderBookResponse GetOrderBook(global::Tinkoff.InvestApi.V1.GetOrderBookRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetOrderBook(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///Метод получения стакана по инструменту.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Tinkoff.InvestApi.V1.GetOrderBookResponse GetOrderBook(global::Tinkoff.InvestApi.V1.GetOrderBookRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_GetOrderBook, null, options, request);
      }
      /// <summary>
      ///Метод получения стакана по инструменту.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Tinkoff.InvestApi.V1.GetOrderBookResponse> GetOrderBookAsync(global::Tinkoff.InvestApi.V1.GetOrderBookRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetOrderBookAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///Метод получения стакана по инструменту.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Tinkoff.InvestApi.V1.GetOrderBookResponse> GetOrderBookAsync(global::Tinkoff.InvestApi.V1.GetOrderBookRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_GetOrderBook, null, options, request);
      }
      /// <summary>
      ///Метод запроса статуса торгов по инструментам.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Tinkoff.InvestApi.V1.GetTradingStatusResponse GetTradingStatus(global::Tinkoff.InvestApi.V1.GetTradingStatusRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetTradingStatus(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///Метод запроса статуса торгов по инструментам.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Tinkoff.InvestApi.V1.GetTradingStatusResponse GetTradingStatus(global::Tinkoff.InvestApi.V1.GetTradingStatusRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_GetTradingStatus, null, options, request);
      }
      /// <summary>
      ///Метод запроса статуса торгов по инструментам.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Tinkoff.InvestApi.V1.GetTradingStatusResponse> GetTradingStatusAsync(global::Tinkoff.InvestApi.V1.GetTradingStatusRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetTradingStatusAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///Метод запроса статуса торгов по инструментам.
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Tinkoff.InvestApi.V1.GetTradingStatusResponse> GetTradingStatusAsync(global::Tinkoff.InvestApi.V1.GetTradingStatusRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_GetTradingStatus, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected override MarketDataServiceClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new MarketDataServiceClient(configuration);
      }
    }

  }
  public static partial class MarketDataStreamService
  {
    static readonly string __ServiceName = "tinkoff.public.invest.api.contract.v1.MarketDataStreamService";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Tinkoff.InvestApi.V1.MarketDataRequest> __Marshaller_tinkoff_public_invest_api_contract_v1_MarketDataRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Tinkoff.InvestApi.V1.MarketDataRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Tinkoff.InvestApi.V1.MarketDataResponse> __Marshaller_tinkoff_public_invest_api_contract_v1_MarketDataResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Tinkoff.InvestApi.V1.MarketDataResponse.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Tinkoff.InvestApi.V1.MarketDataRequest, global::Tinkoff.InvestApi.V1.MarketDataResponse> __Method_MarketDataStream = new grpc::Method<global::Tinkoff.InvestApi.V1.MarketDataRequest, global::Tinkoff.InvestApi.V1.MarketDataResponse>(
        grpc::MethodType.DuplexStreaming,
        __ServiceName,
        "MarketDataStream",
        __Marshaller_tinkoff_public_invest_api_contract_v1_MarketDataRequest,
        __Marshaller_tinkoff_public_invest_api_contract_v1_MarketDataResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Tinkoff.InvestApi.V1.MarketdataReflection.Descriptor.Services[1]; }
    }

    /// <summary>Client for MarketDataStreamService</summary>
    public partial class MarketDataStreamServiceClient : grpc::ClientBase<MarketDataStreamServiceClient>
    {
      /// <summary>Creates a new client for MarketDataStreamService</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public MarketDataStreamServiceClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for MarketDataStreamService that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public MarketDataStreamServiceClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected MarketDataStreamServiceClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected MarketDataStreamServiceClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>
      ///Bi-directional стрим предоставления биржевой информации.
      /// </summary>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncDuplexStreamingCall<global::Tinkoff.InvestApi.V1.MarketDataRequest, global::Tinkoff.InvestApi.V1.MarketDataResponse> MarketDataStream(grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return MarketDataStream(new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///Bi-directional стрим предоставления биржевой информации.
      /// </summary>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncDuplexStreamingCall<global::Tinkoff.InvestApi.V1.MarketDataRequest, global::Tinkoff.InvestApi.V1.MarketDataResponse> MarketDataStream(grpc::CallOptions options)
      {
        return CallInvoker.AsyncDuplexStreamingCall(__Method_MarketDataStream, null, options);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected override MarketDataStreamServiceClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new MarketDataStreamServiceClient(configuration);
      }
    }

  }
}
#endregion
